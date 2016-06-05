﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using log4net;
using L2dotNET.GameService.Enums;
using L2dotNET.GameService.templates;

namespace L2dotNET.GameService.tables
{
    sealed class CharTemplateTable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CharTemplateTable));

        private static volatile CharTemplateTable instance;
        private static readonly object syncRoot = new object();

        public Dictionary<int, PcTemplate> Templates { get; } = new Dictionary<int, PcTemplate>();

        public static CharTemplateTable Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new CharTemplateTable();
                        }
                    }
                }

                return instance;
            }
        }

        public CharTemplateTable()
        {

        }

        public void Initialize()
        {
            XmlDocument doc = new XmlDocument();
            string[] xmlFilesArray = Directory.GetFiles(@"data\xml\classes\");
            for (int i = 0; i < xmlFilesArray.Length; i++)
            {
                doc.Load(xmlFilesArray[i]);
                XmlNodeList nodes = doc.DocumentElement.SelectNodes("/list/class");

                foreach (XmlNode node in nodes)
                {

                    if ("class".Equals(node.Attributes[0].OwnerElement.Name))
                    {
                        XmlNamedNodeMap attrs = node.Attributes;
                        ClassId classId = ClassId.Values.FirstOrDefault(x => ((int)x.Id).Equals(Convert.ToInt32(attrs.Item(0).Value)));
                        StatsSet set = new StatsSet();

                        for (XmlNode cd = node.FirstChild; cd != null; cd = cd.NextSibling)
                        {

                            if ("set".Equals(cd.NextSibling.Name) && cd.NextSibling != null)
                            {
                                attrs = cd.NextSibling.Attributes;
                                string name = attrs.GetNamedItem("name").Value;
                                string value = attrs.GetNamedItem("val").Value;
                                set.Set(name, value);
                            }
                            else
                                break;
                        }
                        PcTemplate pcTempl = new PcTemplate(classId, set);
                        Templates.Add((int)pcTempl.ClassId.Id, pcTempl);
                        
                    }

                }


            }
            Log.Info($"Loaded { Templates.Count } character templates.");
        }

        public PcTemplate GetTemplate(ClassIds classId)
        {
            return Templates[(int)classId];
        }

        public PcTemplate GetTemplate(int classId)
        {
            return Templates[classId];
        }

    }
}
