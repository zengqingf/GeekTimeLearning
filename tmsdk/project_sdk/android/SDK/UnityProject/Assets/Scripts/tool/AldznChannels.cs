using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class SDKChannelLocalInfo : ScriptableObject
{
    public string loginIp;
    public string loginPort;
    public string payIp;
    public string payPort;
    public string channelName;
    public string serverId;
    public string payPrice;
}

public class ServerMarkAttribute : Attribute
{
    public string ServerMark { get; set; }
    public ServerMarkAttribute(string mark)
    {
        this.ServerMark = mark;
    }
}

public class RemarkAttribute : Attribute
{
    public string Remark { get; set; }
    public RemarkAttribute(string remark)
    {
        this.Remark = remark;
    }
}

public class RemarkFloatAttribute : Attribute
{
    public float RemarkFloat {get;set;}
    public RemarkFloatAttribute(float floatV)
    {
        this.RemarkFloat = floatV;
    }
}

public class RemarkIntAttribute : Attribute
{
    public int RemarkInt {get;set;}
    public RemarkIntAttribute(int intV)
    {
        this.RemarkInt = intV;
    }
}

public class RemarkStringArrayAttribute : Attribute
{
    public string[] RemarkStringArray {get;set;}
    public RemarkStringArrayAttribute(string[] strArray)
    {
        this.RemarkStringArray = strArray; 
    }
}

public class RemarkStringAttribute : Attribute
{
    public string RemarkString {get;set;}
    public RemarkStringAttribute(string str)
    {
        this.RemarkString = str; 
    }
}

public class RemarkBooleanAttribute : Attribute
{
    public bool RemarkBoolean { get; set; }
    public RemarkBooleanAttribute(bool _boolean)
    {
        this.RemarkBoolean = _boolean;
    }
}

public static class EnumExtension
{
    public static string GetServerMark<T>(this T value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());
        if (fi == null)
        {
            return value.ToString();
        }
        object[] attributes = fi.GetCustomAttributes(typeof(ServerMarkAttribute), false);
        if (attributes != null && attributes.Length > 0)
        {
            return ((ServerMarkAttribute)attributes[0]).ServerMark;
        }
        else
        {
            return value.ToString();
        }
    }

    public static string GetRemark<T>(this T value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());
        if (fi == null)
        {
            return value.ToString();
        }
        object[] attributes = fi.GetCustomAttributes(typeof(RemarkAttribute), false);
        if (attributes != null && attributes.Length > 0)
        {
            return ((RemarkAttribute)attributes[0]).Remark;
        }
        else
        {
            return value.ToString();
        }
    }

    public static int GetRemarkInt<T>(this T value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());
        if (fi == null)
        {
            return 1;
        }
        object[] attributes = fi.GetCustomAttributes(typeof(RemarkIntAttribute), false);
        if (attributes != null && attributes.Length > 0)
        {
            return ((RemarkIntAttribute)attributes[0]).RemarkInt;
        }
        else
        {
            return 1;
        }
    }

    public static float GetRemarkFloat<T>(this T value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());
        if (fi == null)
        {
            return 1.0f;
        }
        object[] attributes = fi.GetCustomAttributes(typeof(RemarkFloatAttribute), false);
        if (attributes != null && attributes.Length > 0)
        {
            return ((RemarkFloatAttribute)attributes[0]).RemarkFloat;
        }
        else
        {
            return 1.0f;
        }
    }

    public static string[] GetRemarkStringArray<T>(this T value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());
        if (fi == null)
        {
            return null;
        }
        object[] attributes = fi.GetCustomAttributes(typeof(RemarkStringArrayAttribute), false);
        if (attributes != null && attributes.Length > 0)
        {
            return ((RemarkStringArrayAttribute)attributes[0]).RemarkStringArray;
        }
        else
        {
            return null;
        }
    }

    public static string GetRemarkString<T>(this T value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());
        if (fi == null)
        {
            return "";
        }
        object[] attributes = fi.GetCustomAttributes(typeof(RemarkStringAttribute), false);
        if (attributes != null && attributes.Length > 0)
        {
            return ((RemarkStringAttribute)attributes[0]).RemarkString;
        }
        else
        {
            return "";
        }
    }

    public static bool GetRemarkBoolean<T>(this T value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());
        if (fi == null)
        {
            return false;
        }
        object[] attributes = fi.GetCustomAttributes(typeof(RemarkBooleanAttribute), false);
        if (attributes != null && attributes.Length > 0)
        {
            return ((RemarkBooleanAttribute)attributes[0]).RemarkBoolean;
        }
        else
        {
            return false;
        }
    }

    public static string GetDescriptionByName<T>(this T enumItemName)
    {
        FieldInfo fi = enumItemName.GetType().GetField(enumItemName.ToString());
        if (fi == null)
        {
            return enumItemName.ToString();
        }
        DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (attributes != null && attributes.Length > 0)
        {
            return attributes[0].Description;
        }
        else
        {
            return enumItemName.ToString();
        }
    }
}