using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Globalization;
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

    public float zoomUpdateDelay = 0.01f;

    private float targetZoom;
    private float lastZoomTime;

    void Start()
    {
        targetZoom = zoom;
        lastZoomTime = Time.time;
        StartCoroutine(GetMapbox());
        rect = gameObject.GetComponent<RawImage>().rectTransform.rect;
        mapWidth = (int)Math.Round(rect.width);
        mapHeight = (int)Math.Round(rect.height);
    }

    void Update()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            targetZoom += scrollInput * 2.0f;
            targetZoom = Mathf.Clamp(targetZoom, 12.0f, 20.0f);
        }

        if (Time.time - lastZoomTime >= zoomUpdateDelay)
        {
            zoom = targetZoom;

            url = "https://api.mapbox.com/styles/v1/mapbox/" + styleStr[(int)mapStyle] + "/static/" +
                  centerLongitude.ToString("F4", CultureInfo.InvariantCulture) + "," +
                  centerLatitude.ToString("F4", CultureInfo.InvariantCulture) + "," +
                  zoom.ToString("F4", CultureInfo.InvariantCulture) + "," +
                  bearing.ToString("F4", CultureInfo.InvariantCulture) + " /" +
                  mapWidth + "x" + mapHeight + "?" + "access_token=" + accessToken;

            Debug.Log("Generated URL: " + url);

            StartCoroutine(GetMapbox());
            lastZoomTime = Time.time;
        }
    }

    IEnumerator GetMapbox()
    {
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
