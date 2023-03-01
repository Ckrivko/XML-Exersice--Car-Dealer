using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.Data
{
    public static class Helper
    {
        internal static T Deserialize<T>(string inputXml, string rootName)
        {
            XmlRootAttribute root = new XmlRootAttribute(rootName);
            var serializer = new XmlSerializer(typeof(T), root);


            T dtos = (T)serializer.Deserialize(new StringReader(inputXml));

            return dtos;
        }

        internal static string Serializer<T>(T dto, string rootName)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute root = new XmlRootAttribute(rootName);
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            var xmlSerializer = new XmlSerializer(typeof(T), root);
            using StringWriter writer = new StringWriter(sb);

            xmlSerializer.Serialize(writer, dto, namespaces);

            return sb.ToString().Trim();
        }

        internal static string InitializeFilePath(string fileName,string filePath)
        {
            filePath = Path.Combine(Directory.GetCurrentDirectory(), "../../../Datasets/", fileName);
            return filePath;
        }
        internal static string InitializeOutputFilePath(string fileName, string filePath)
        {
            filePath = Path.Combine(Directory.GetCurrentDirectory(), "../../../Results/", fileName);
            return filePath;
        }

        internal static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult);

            return isValid;
        }


    }
}
