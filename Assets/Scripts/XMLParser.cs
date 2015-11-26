﻿using System.Xml.Serialization;
using System.Xml;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class XMLParser {

    XmlSerializer serializer = new XmlSerializer(typeof(Level));

    public object parse(string filename)
    {
        TextAsset textAsset = (TextAsset)Resources.Load(
                                            ScenesParameters.LevelsDirectory +
                                            '/' + ScenesParameters.Section +
                                             '/' + filename);
        
        System.IO.StringReader stringReader = new System.IO.StringReader(textAsset.text);
        System.Xml.XmlReader reader = System.Xml.XmlReader.Create(stringReader);

        return (Level)serializer.Deserialize(reader);
    }

    public void makeLevel(Level level)
    {
        var serializationFile = Path.Combine(Directory.GetCurrentDirectory(),
                                            "Assets"
                                            + Path.DirectorySeparatorChar +
                                            "Resources" + Path.DirectorySeparatorChar +
                                            ScenesParameters.LevelsDirectory + Path.DirectorySeparatorChar 
                                            + ScenesParameters.Section + 
                                            Path.DirectorySeparatorChar + ScenesParameters.LevelName 
                                            + ScenesParameters.LevelsNumber + ".xml");

        Debug.Log(serializationFile);

        using (var writer = XmlWriter.Create(serializationFile))
        {
            serializer.Serialize(writer, level);
        }
    }

    public void makeLevelFromCurrentState()
    {
        var balls = GameObject.FindGameObjectsWithTag("Ball");
        //  ^^^^^ LOL

        var baskets = GameObject.FindGameObjectsWithTag("Basket");

        var triangleObsticles = GameObject.FindGameObjectsWithTag("TriangleObsticle");

        GameObject inputFieldGo = GameObject.Find("RequiredInputField");
        var inputFieldCo = inputFieldGo.GetComponent<InputField>();
        Debug.Log(inputFieldCo);

        if (balls.Length > 0 && baskets.Length > 0)
        {
            ++ScenesParameters.LevelsNumber;
            var triangleObsticle = triangleObsticles.Length > 0 ? triangleObsticles[0] : null;
            Level level = new Level(balls[0], baskets[0], triangleObsticle, inputFieldCo.text);
            makeLevel(level);

            var configPass = Path.Combine(Directory.GetCurrentDirectory(),
                                            "Assets" + Path.DirectorySeparatorChar +
                                            "Resources"
                                            + Path.DirectorySeparatorChar +
                                            ScenesParameters.LevelsDirectory 
                                            + Path.DirectorySeparatorChar
                                            + ScenesParameters.Section +
                                            Path.DirectorySeparatorChar + "config.txt");

            FileStream fcreate = File.Open(configPass, FileMode.Create);
            var stream = new StreamWriter(fcreate);
            stream.WriteLine(ScenesParameters.LevelsNumber);
            stream.Close();
            fcreate.Close();
        } else
        {
            Debug.Log("Ball and Basket are required");
        }
    }
}
