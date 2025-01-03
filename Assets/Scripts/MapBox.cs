using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Xml;
using System;

public class Mapbox : MonoBehaviour
{
    public string accessToken = "pk.eyJ1IjoiYmVlYyIsImEiOiJjbTVmc3RldDcwMHpjMmpzNnRldGJvZ3Q3In0.NT7SvmrXKIG8kE4cFX5eMg";
    public float centerLongitude = 19.2892f;
    public float centerLatitude = 48.0716f;
    public float zoom = 12.0f;
    public int bearing = 0;
    public enum style { Light, Dark, Streets, Outdoors, Satellite, SatelliteStreets };
    public style mapStyle = style.Streets;
    public enum resolution { low = 1, high = 2 };
    public resolution mapResolution = resolution.low;

    private int mapWidth = 500;
    private int mapHeight = 300;
    private string[] styleStr = new string[] { "light-v11", "dark-v11", "streets-v12", "outdoors-v12", "satellite-v9", "satellite-streets-v12" };
    private string url = "";
    private bool mapIsLoading = false;
    private Rect rect;
    private bool updateMap = true;

    private string accessTokenLast;
    private float centerLongitudeLast = 19.2892f;
    private float centerLatitudeLast = 48.0716f;
    private float zoomLast = 12.0f;
    private int bearingLast = 0;
    private style mapStyleLast = style.Streets;
    private resolution mapResolutionLast = resolution.low;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetMapbox());
        rect = gameObject.GetComponent<RawImage>().rectTransform.rect;
        mapWidth = (int)Math.Round(rect.width);
        mapHeight = (int)Math.Round(rect.height);
    }

    // Update is called once per frame
    void Update()
    {
        if (updateMap && (accessTokenLast != accessToken || !Mathf.Approximately(centerLatitudeLast, centerLatitude) || !Mathf.Approximately(centerLongitudeLast, centerLongitude) || zoomLast != zoom || bearingLast != bearing || mapStyleLast != mapStyle || mapResolutionLast != mapResolution))
        {
            rect = gameObject.GetComponent<RawImage>().rectTransform.rect;
            mapWidth = (int)Math.Round(rect.width);
            mapHeight = (int)Math.Round(rect.height);
            StartCoroutine(GetMapbox());
            updateMap = false;
        }
    }


    IEnumerator GetMapbox()
    {
        //url = "https://api.mapbox.com/styles/v1/mapbox/streets-v12/static/19.2892,48.0716,12,0/500x300?access_token=" + accessToken;
        //url = "https://api.mapbox.com/styles/v1/mapbox/" + styleStr[(int)mapStyle] + "/static/" + centerLongitude + "," + centerLatitude + "," + zoom + "," + bearing + "/" + mapWidth + "x" + mapHeight + "?" + "access_token=" + accessToken;
        url = "https://api.mapbox.com/styles/v1/mapbox/" + styleStr[(int)mapStyle] + "/static/" + "19.2892" + "," + "48.0716" + "," + zoom + "," + bearing + " /" + mapWidth + "x" + mapHeight + "?" + "access_token=" + accessToken;
        mapIsLoading = true;
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("WWW ERROR: " + www.error);
        }
        else
        {
            mapIsLoading = false;
            gameObject.GetComponent<RawImage>().texture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            accessTokenLast = accessToken;
            centerLatitudeLast = centerLatitude;
            centerLongitudeLast = centerLongitude;
            zoomLast = zoom;
            bearingLast = bearing;
            mapStyleLast = mapStyle;
            mapResolutionLast = mapResolution;
            updateMap = true;
        }
    }
}