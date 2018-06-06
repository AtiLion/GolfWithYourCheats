using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace CheatModule.API.Config
{
    public static class ConfigManager
    {
        #region Public Constants

        public static readonly string Config_Path = Application.persistentDataPath + "/" + CheatConfig.ConfigName + ".xml";

        #endregion Public Constants

        #region Public Properties

        public static Dictionary<string, object> Configuration { get; private set; } = new Dictionary<string, object>();

        #endregion Public Properties

        static ConfigManager()
        {
            if (File.Exists(Config_Path))
                Load();
        }

        #region Private Functions

        private static void HandleNode(XmlNode node)
        {
            if (node.ParentNode != null)
                Configuration.Add(node.Name, Convert.ChangeType(node.Value, Type.GetType(node.Attributes["type"].Value + ", " + node.Attributes["assembly"].Value)));

            if (node.HasChildNodes)
                foreach (XmlNode child in node.ChildNodes)
                    HandleNode(child);
        }

        #endregion Private Functions

        #region Public Functions

        public static void Load()
        {
            XmlDocument document = new XmlDocument();
            document.Load(Config_Path);

            HandleNode(document.DocumentElement);
        }

        public static void Save()
        {
            XmlDocument document = new XmlDocument();
            XmlNode root = document.CreateElement("config");

            document.AppendChild(root);
            foreach (string key in Configuration.Keys)
            {
                XmlNode config = document.CreateElement(key);
                XmlAttribute typeAttribute = document.CreateAttribute("type");
                XmlAttribute assemblyAttribute = document.CreateAttribute("assembly");

                typeAttribute.Value = Configuration[key].GetType().FullName;
                assemblyAttribute.Value = Configuration[key].GetType().AssemblyQualifiedName;
                config.Value = Configuration[key].ToString();

                config.Attributes.Append(typeAttribute);
                config.Attributes.Append(assemblyAttribute);
                root.AppendChild(config);
            }
            document.Save(Config_Path);
        }

        public static object Get(string name)
        {
            if (!Configuration.ContainsKey(name))
                return null;

            return Configuration[name];
        }

        public static T Get<T>(string name) =>
            (T)Get(name);

        public static bool Set(string name, object value)
        {
            if (Configuration.ContainsKey(name))
                return false;

            Configuration.Add(name, value);
            return true;
        }

        #endregion Public Functions
    }
}