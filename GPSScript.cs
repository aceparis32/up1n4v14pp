using UnityEngine;
using System.Collections;
using System;
using Mono.Data.Sqlite;
using System.Data;
using UnityEngine.UI;
using System.IO;

public class GPSScript : MonoBehaviour {

    public GameObject pic3D;
    private double distanceTemp = 0;
    public Text location;
    public Text lat;
    public Text lon;
    public Text distance;
    public TextMesh webPlaceName;
    public Image compassImage;
    public int idInfo;

    void Start()
    {
        //respawns = GameObject.FindGameObjectsWithTag("facultyInfo");

        //pic3D.SetActive(false);
        // test on PC
        string _constr = Application.persistentDataPath + "/" + "skripsiDB.db";
        // on Android
        if (!File.Exists(_constr))
        {
            lon.text = "file baru";
            WWW getDB = new WWW("jar:file://" + Application.dataPath + "!/assets/" + "skripsiDB.db");

            while (!getDB.isDone)
            {
            }

            File.WriteAllBytes(_constr, getDB.bytes);
        }
        
        string conn = "URI=file:" + _constr;
        //WWW dbPath = new WWW(path);
        IDbConnection _dbc;
        _dbc = new SqliteConnection(conn);
        _dbc.Open();
        IDbCommand _dbcm = _dbc.CreateCommand();
        _dbcm.CommandText = "select * " + "FROM 'information'";
        IDataReader _dbr = _dbcm.ExecuteReader();

        StartCoroutine(startSearchingPlace(_dbr));
    }

    IEnumerator startSearchingPlace(IDataReader dbr)
    {
        while (true)
        {
            // First, check if user has location service enabled
            if (!Input.location.isEnabledByUser)
                yield break;

            // Start service before querying location
            Input.location.Start(1f,1f);
            Input.compass.enabled = true;

            // Wait until service initializes
            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            // Service didn't initialize in 20 seconds
            if (maxWait < 1)
            {
                print("Timed out");
                yield break;
            }

            // Connection has failed
            if (Input.location.status == LocationServiceStatus.Failed)
            {
                print("Unable to determine device location");
                yield break;
            }
            else
            {
                compassImage.transform.localRotation = Quaternion.Euler(0, 0, Input.compass.trueHeading);
                this.transform.rotation = Quaternion.Euler(0, Input.compass.trueHeading, 0);
                
                while (dbr.Read())
                {
                    double convDouble = Convert.ToDouble(Input.location.lastData.longitude);
                    var roundDouble = Math.Round(convDouble, 6);
                    var jarak = getDistance(Input.location.lastData.latitude, roundDouble, dbr.GetDouble(2), dbr.GetDouble(3));
                    //lat.text = Input.acceleration.x + "\\" + Input.acceleration.y;
                    lon.text = Input.location.lastData.longitude.ToString("R") + "===" + roundDouble.ToString() + "-" + Input.location.lastData.longitude;
                    distance.text = distance.text + "//" + jarak.ToString();
                    //Debug.Log(distance);
                    if (jarak <= 30)
                    {
                        // jika ditemukan lebih dari 1 data, maka pilih yang jaraknya paling dekat
                        if (distanceTemp == 0)
                        {
                            distanceTemp = jarak;
                        }

                        if (jarak <= distanceTemp)
                        {
                            pic3D.SetActive(true);
                            distanceTemp = jarak;
                            //distance.text = jarak.ToString();
                            location.text = "Lokasi : " + dbr.GetString(4);
                            idInfo = dbr.GetInt32(0);
                            string getDescription = dbr.GetString(4);
                            string getidinfo = dbr.GetInt32(0).ToString();
                            
                            //respawns[0].GetComponent<TextMesh>().text = getDescription;
                            //respawns[0].GetComponent<TextMesh>().text = getidinfo;
                            //IDbCommand dbcm = dbc.CreateCommand();
                            //dbcm.CommandText = "select * " + "from 'faculty' where 'idInfo' = " + dbr.GetData(0);
                            //dbr = dbcm.ExecuteReader();
                            //while (dbr.Read())
                            //{
                            //    respawns[0].GetComponent<TextMesh>().text = dbr.GetData(2).ToString();
                            //    respawns[1].GetComponent<TextMesh>().text = dbr.GetData(3).ToString();
                            //}
                        }
                    }
                }
            }
        }
        // Stop service if there is no need to query location updates continuously
        //Input.location.Stop();
    }

    double getDistance(double latPlayer, double lonPlayer, double latTarget, double lonTarget){
            var R = 6371.137; // Radius of the earth in km
            // latTarget - latPlayer
            var dLat = latPlayer * Math.PI / 180 - latTarget * Math.PI / 180;
            var dLon = lonPlayer * Math.PI / 180 - lonTarget * Math.PI / 180;
            var a =
              Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
              Math.Cos(latTarget * Math.PI / 180) * Math.Cos(latPlayer * Math.PI / 180) *
              Math.Sin(dLon / 2) * Math.Sin(dLon / 2)
              ;
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = (R * c) * 1000f; // Distance in m
            return d;
    }

    double deg2rad(double deg)
    {
        return deg * (Math.PI / 180);
    }

    

    // Update is called once per frame
    //void Update () {

    //}
}
