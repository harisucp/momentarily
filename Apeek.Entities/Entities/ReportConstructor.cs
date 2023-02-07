using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace Apeek.Entities.Entities
{
    [Serializable]
    [DataContract]
    public class ReportConstructor
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string ReportName { get; set; }
        [DataMember]
        public string MainSQL { get; set; }
        [DataMember]
        public List<ReportComponent> Components { get; set; }
        public struct ComponentsType
        {
            public const string text = "текст";
            public const string data = "дата";
            public const string list = "список";
            public const string button = "кнопка";
        }
        public ReportConstructor()
        {
            Components = new List<ReportComponent>();
        }
        public ReportConstructor(string strRepName, string strMainSQL, string strComText, string strComName, string strComType)
        {
            Components = new List<ReportComponent>();
            ReportName = strRepName;
            MainSQL = strMainSQL;
            Components[Components.Count - 1].Text = strComText;
            Components[Components.Count - 1].Name = strComName;
            Components[Components.Count - 1].Type = strComType;
        }
        public void Clear()
        {
            Components.Clear();
        }
    }
    [DataContract]
    public class ReportComponent
    {
        [DataMember]
        public string Text { get; set; }//текст відображається біля компонента
        [DataMember]
        public string Name { get; set; }//назва компонента і назва змінної
        [DataMember]
        public string Type { get; set; }//тип(список,кнопка,дата...)
        [DataMember]
        public string Value { get; set; }//тип(список,кнопка,дата...)
        [DataMember]
        public ReportComponentProperty ComProperty { get; set; }
        public ReportComponent() { }
        public ReportComponent(string strComText, string strComName, string strComType)
        {
            Text = strComText;
            Name = strComName;
            Type = strComType;
        }
    }
    [DataContract]
    public class ReportComponentProperty
    {
        [DataMember]
        public string Sql { get; set; }
        [DataMember]
        public string KeyIndex { get; set; }
        [DataMember]
        public string DisplayIndex { get; set; }
    }
}