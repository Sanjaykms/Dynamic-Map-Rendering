using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using TMPro;
using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Microsoft.MixedReality.Toolkit.UI;
public class Controller : MonoBehaviour
{
    //Declerations needed
    public MapRenderer map;
    public ParticleSystem rain;
    public TMP_InputField inputField;
    public TMP_Text city;
    public TMP_Text time;
    public TMP_Text symbolPhrase;
    public TMP_Text temperature;
    public TMP_Text feelsLikeTemp;
    public TMP_Text relHumidity;
    public TMP_Text windSpeed;
    public TMP_Text windDirString;
    public TMP_Text windGust;
    public TMP_Text thunderProb;
    public TMP_Text cloudiness;
    public TMP_Text pressure;
    String cityName = "tokyo";
    DynamicWeather jobj;
    WeatherDetails kobj;
    // Start is called before the first frame update
    public void ValChange()
    {
        if ((inputField.text != "" || cityName==inputField.text)&& !Regex.IsMatch(inputField.text, @"^\d+$"))
        {
            cityName = inputField.text;
        }
        Start();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    async void Start()
    {
        rain.gameObject.SetActive(false);
        var client = new HttpClient();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://foreca-weather.p.rapidapi.com/location/search/"+cityName),
            Headers = {
                         { "X-RapidAPI-Key", "6d21b870e5msheb67be3cb5c83c4p14975cjsndf7291e88085" },
                         { "X-RapidAPI-Host", "foreca-weather.p.rapidapi.com" },
            },
        };
        using (var response = await client.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            Debug.Log(body.ToString());
            jobj = JsonConvert.DeserializeObject<DynamicWeather>(body);
            Debug.Log(jobj.locations[0].id);
            AnimateToPlace(jobj.locations[0].lat, jobj.locations[0].lon);
            WeatherHandler();
        }
    }
    async void WeatherHandler()
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://foreca-weather.p.rapidapi.com/current/"+jobj.locations[0].id),
            Headers =
    {
        { "X-RapidAPI-Key", "6d21b870e5msheb67be3cb5c83c4p14975cjsndf7291e88085" },
        { "X-RapidAPI-Host", "foreca-weather.p.rapidapi.com" },
    },
        };
        using (var response = await client.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            Debug.Log(body);
            kobj = JsonConvert.DeserializeObject<WeatherDetails>(body);
            Debug.Log(kobj.current.cloudiness);
            city.text = "City : "+cityName;
            time.text = "Time : " + kobj.current.time;
            symbolPhrase.text = "Symbol Phrase : " + kobj.current.symbolPhrase;
            temperature.text = "Temperature : " + kobj.current.temperature;
            feelsLikeTemp.text = "Feels Like Temp : " + kobj.current.feelsLikeTemp;
            relHumidity.text = "Rel Humidity : " + kobj.current.relHumidity;
            windSpeed.text = "Wind Speed : " + kobj.current.windSpeed;
            windDirString.text = "Wind Dir String : " + kobj.current.windDirString;
            windGust.text = "Wind Gust : " + kobj.current.windGust;
            thunderProb.text = "Thunder Prob : " + kobj.current.thunderProb;
            cloudiness.text = "Cloudiness : " + kobj.current.cloudiness;
            pressure.text = "Pressure : " + kobj.current.pressure;
            if (kobj.current.symbolPhrase=="showers" || kobj.current.symbolPhrase == "thunderstorms" || kobj.current.symbolPhrase == "light rain" || kobj.current.symbolPhrase == "heavy rain" || kobj.current.symbolPhrase == "rain" || kobj.current.symbolPhrase == "high rain")
            {
                Debug.Log("rain");
                rain.gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("no rain");
                rain.gameObject.SetActive(false);
            }
        }
    }
    public void MapZoomHandler(SliderEventData eventData)
    {
        /*map.ZoomLevel = slid.value;
        Debug.Log(slid.value);*/
         Debug.Log(eventData.NewValue);
        map.ZoomLevel = eventData.NewValue * 20;
    }
    public void ModelsToggleHandler(bool toggle)
    {
        map.MapTerrainType = toggle ? MapTerrainType.Default : MapTerrainType.Elevated;
    }
    public void MapShapeHandler(int shape)
    {
        if (shape == 0)
        {
            map.MapShape = MapShape.Block;
        }
        else
        {
            map.MapShape = MapShape.Cylinder;
        }
    }
    void AnimateToPlace(float a,float b)
    {
        map.SetMapScene(new MapSceneOfLocationAndZoomLevel(new LatLon(a, b), 16f));
    }

}
//Json Decode
public class DynamicWeather
{
    public Class1[] locations { get; set; }
}
public class Class1
{
    public int id { get; set; }
   /* public string name { get; set; }
    public string country { get; set; }
    public string timezone { get; set; }
    public string language { get; set; }
    public string adminArea { get; set; }
    public string adminArea2 { get; set; }
    public string adminArea3 { get; set; }*/
    public float lon { get; set; }
    public float lat { get; set; }
}
//Weather Details
public class WeatherDetails
{
    public DetailsWe current { get; set; }
}
public class DetailsWe
{
    public string time { get; set; }
    public string symbolPhrase { get; set; }
    public float temperature { get; set; }
    public float feelsLikeTemp { get; set; }
    public float relHumidity { get; set; }
    public float windSpeed { get; set; }
    public string windDirString { get; set; }
    public float windGust { get; set; }
    public float thunderProb { get; set; }
    public float cloudiness { get; set; }
    public float pressure { get; set; }
}
