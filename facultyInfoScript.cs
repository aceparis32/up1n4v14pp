using UnityEngine;
using System.Collections;
using System;
using Mono.Data.Sqlite;
using System.Data;
using UnityEngine.UI;
using System.IO;

public class facultyInfoScript : MonoBehaviour {

    public GameObject[] respawns;

    // Use this for initialization
    void Start () {
        GameObject getGameObject = GameObject.Find("Camera");
        GPSScript gpsScript = getGameObject.GetComponent<GPSScript>();
        respawns = GameObject.FindGameObjectsWithTag("facultyInfo");

        string _constr = Application.persistentDataPath + "/" + "skripsiDB.db";
        // on Android
        //if (!File.Exists(_constr))
        //{
        //    WWW getDB = new WWW("jar:file://" + Application.dataPath + "!/assets/" + "skripsiDB.db");

        //    while (!getDB.isDone)
        //    {
        //    }

        //    File.WriteAllBytes(_constr, getDB.bytes);
        //}
        
        string conn = "URI=file:" + _constr;
        //WWW dbPath = new WWW(path);
        IDbConnection _dbc;
        _dbc = new SqliteConnection(conn);
        _dbc.Open();
        IDbCommand _dbcm = _dbc.CreateCommand();
        _dbcm.CommandText = "select * " + "FROM 'faculty' WHERE idInfo = " + gpsScript.idInfo;
        IDataReader _dbr = _dbcm.ExecuteReader();

        while (_dbr.Read())
        {
            respawns[0].GetComponent<TextMesh>().text = _dbr.GetString(2);
            respawns[1].GetComponent<TextMesh>().text = _dbr.GetString(3);
        }

        //_dbr.Close();
        //_dbc.Close();
    }

    // Update is called once per frame
    //void Update()
    //{

    //}
}
