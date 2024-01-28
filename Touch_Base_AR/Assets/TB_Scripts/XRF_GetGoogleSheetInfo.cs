using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Assertions;
using System.Linq;
using System;
using System.Threading;

public class XRF_GetGoogleSheetInfo : MonoBehaviour
{
    //download test
    //https://docs.google.com/spreadsheets/d/1gseHmqvgk097HVn41hLluQeEiBrECGQ4L55GYhmhN5w/export?format=csv

    [Header("Google Sheet URLs")]
    [Tooltip("This is the END of the URL for the Username and Pin sheet of all employees. To get this, click 'get shareable link', copy the link, and copy the segment BETWEEN '/d/' and '/edit?usp=sharing'.")]
    public string URL_GoogleSheet_ProjectInformation = "https://docs.google.com/spreadsheets/d/1gseHmqvgk097HVn41hLluQeEiBrECGQ4L55GYhmhN5w/edit?usp=sharing";

    private List<string> studentNames = new List<string>();
    private List<string> projectNames = new List<string>();
    private List<string> projectURLs = new List<string>();
    private List<string> DiagramURLS = new List<string>();

    

    [Header("Scroll View Properties and References")]
    public float downloadYValueOriginal = -30.0f;
    public float downloadYPadding = 90.0f;
    public GameObject downloadButtonPrefab;
    public GameObject contentHolder;

    private List<GameObject> currentModelList = new List<GameObject>();
    private float downloadYValue = 0.0f;
    private GameObject downloadButton;


    private void Start()
    {
        downloadButtonPrefab.SetActive(false);
    }

    public void Button_GetUsernames()
    {
        GetAllUsernames();

    }

    //need a refresh button in case someone uploads while they have the app open

    private void GetAllUsernames()
    {
        studentNames = new List<string>();
        projectNames = new List<string>();
        projectURLs = new List<string>();
        DiagramURLS = new List<string>();

        //this gets the usernams and passwords and locations
        string[] splitURL = URL_GoogleSheet_ProjectInformation.Split("/");
        string k_googleSheetDocID = splitURL[5];
        StartCoroutine(CSVDownloader.DownloadData(k_googleSheetDocID, AfterDownload));
    }

    public void AfterDownload(string data)
    {
        if (null == data)
        {
            Debug.LogError("Was not able to download data or retrieve stale data.");
            // TODO: Display a notification that this is likely due to poor internet connectivity
            //       Maybe ask them about if they want to report a bug over this, though if there's no internet I guess they can't
        }
        else
        {
            StartCoroutine(ProcessData(data));
        }
    }

    public IEnumerator ProcessData(string data)
    {
        yield return new WaitForEndOfFrame();

        // "\r\n" means end of line and should be only occurence of '\r' (unless on macOS/iOS in which case lines ends with just \n)
        char lineEnding = '\r';

        #if UNITY_IOS
        lineEnding = '\n';
#       endif

#       if UNITY_EDITOR
        lineEnding = '\r';
#       endif

        //Debug.Log("data length = " + data.Length);
        string[] dataRows = data.Split(lineEnding);

        string[] rowHeaders = dataRows[0].Split(',');

        int studentNamesInt = 0, projectNamesInt = 0, projectURLsInt = 0, diagramURLsInt = 0;
        for (int i = 0; i < rowHeaders.Length; i++)
        {
            string fixedRowHeader = new string(rowHeaders[i].Where(c => !char.IsControl(c)).ToArray());
            fixedRowHeader = fixedRowHeader.ToLower();

            //Debug.Log("fixedRowHeader = " + fixedRowHeader);

            if (fixedRowHeader == "student name")
            {
                studentNamesInt = i;
            }
            else if (fixedRowHeader == "project name")
            {
                projectNamesInt = i;
            }
            else if (fixedRowHeader == "project url")
            {
                projectURLsInt = i;
            }
            else if (fixedRowHeader == "diagram url")
            {
                diagramURLsInt = i;
            }
        }

        //Debug.Log("studentNamesInt = " + studentNamesInt);
        //Debug.Log("projectNamesInt = " + projectNamesInt);
        //Debug.Log("projectURLsInt = " + projectURLsInt);
        //Debug.Log("diagramURLsInt = " + diagramURLsInt);


        for (int i = 0; i < dataRows.Length; i++)
        {
            string[] dataRowSplit = dataRows[i].Split(',');

            string studentNameOriginal = dataRowSplit[studentNamesInt];
            string studentNameFixed = new string(studentNameOriginal.Where(c => !char.IsControl(c)).ToArray());

            string projectNameOriginal = dataRowSplit[projectNamesInt];
            string projectNameFixed = new string(projectNameOriginal.Where(c => !char.IsControl(c)).ToArray());

            string projectURLOriginal = dataRowSplit[projectURLsInt];
            string projectURLFixed = new string(projectURLOriginal.Where(c => !char.IsControl(c)).ToArray());

            string diaramURLOriginal = dataRowSplit[diagramURLsInt];
            string diaramURLFixed = new string(diaramURLOriginal.Where(c => !char.IsControl(c)).ToArray());


            //Debug.Log("im adding this name = " + studentNameFixed);
            //Debug.Log("im adding this project name = " + projectNameFixed);
            //Debug.Log("im adding this project url = " + projectURLFixed);
            //Debug.Log("im adding this diagram url = " + diaramURLFixed);


            studentNames.Add(studentNameFixed);
            projectNames.Add(projectNameFixed);
            projectURLs.Add(projectURLFixed);
            DiagramURLS.Add(diaramURLFixed);

        }
        PopulateProjectList();
        //populate a scroll view
    }


    void PopulateProjectList()
    {
        for (int i = 0; i < currentModelList.Count; i++)
        {
            Destroy(currentModelList[i]);
        }
        downloadYValue = downloadYValueOriginal;
        currentModelList = new List<GameObject>();

        //start i at 1 because its adding the fixed row header at the top... we dont need this...
        for (int i = 1; i < studentNames.Count; i++)
        {
            downloadButton = Instantiate(downloadButtonPrefab, Vector3.zero, Quaternion.identity);
            //downloadButton.transform.parent = downloadButtonPrefab.transform.parent;
            //downloadButton.GetComponent<RectTransform>().parent = downloadButtonPrefab.GetComponent<RectTransform>().parent;
            RectTransform rt = downloadButton.GetComponent<RectTransform>();
            rt.SetParent(contentHolder.GetComponent<RectTransform>());
            //downloadButton.transform.localPosition = new Vector3(0, downloadYValue, 0);
            rt.localPosition = new Vector3(0, downloadYValue, 0);
            rt.offsetMin = new Vector2(100, rt.offsetMin.y);
            rt.offsetMax = new Vector2(-100, rt.offsetMax.y);

            //downloadButton.transform.localRotation = Quaternion.identity;
            rt.localRotation = Quaternion.identity;

            //downloadButton.transform.localScale = downloadButtonPrefab.transform.localScale;
            rt.localScale = downloadButtonPrefab.GetComponent<RectTransform>().localScale;

            downloadButton.SetActive(true);
            //create a button for each of these and put them into an array
            XRF_ProjectInfoHolder linkComponent = downloadButton.GetComponentInChildren<XRF_ProjectInfoHolder>();
            linkComponent.studentName = studentNames[i];
            linkComponent.projectName = projectNames[i];
            linkComponent.projectURL = projectURLs[i];
            linkComponent.diagramURL = DiagramURLS[i];

            //now we have two...
            TMP_Text[] textObjects = downloadButton.GetComponentsInChildren<TMP_Text>();
            textObjects[0].text = studentNames[i];
            textObjects[1].text = projectNames[i];

            currentModelList.Add(downloadButton);
            downloadYValue = downloadYValue - downloadYPadding;
        }
        //content holder height
        Vector2 textSizeDelta = contentHolder.GetComponent<RectTransform>().sizeDelta;
        textSizeDelta.y = (downloadYValue - downloadYPadding) * -1;
        contentHolder.GetComponent<RectTransform>().sizeDelta = textSizeDelta;
    }
}
