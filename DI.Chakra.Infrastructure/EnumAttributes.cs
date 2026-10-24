using System;

public interface IAttribute
{
    string Value { get; }
}

public class Description : Attribute, IAttribute
{

    private string _value;

    public Description(string text)
    {
        this._value = text;
    }

    public string Value
    {
        get { return _value; }
    }
}

public class Exclude : Attribute, IAttribute
{

    private string _value;

    public Exclude(string text)
    {
        this._value = text;
    }

    public string Value
    {
        get { return _value; }
    }
}

public class DisplayAs : Attribute, IAttribute
{

    private string _value;

    public DisplayAs(string text)
    {
        this._value = text;
    }

    public string Value
    {
        get { return _value; }
    }
}

public class TableName : Attribute, IAttribute
{

    private string _value;

    public TableName(string text)
    {
        this._value = text;
    }

    public string Value
    {
        get { return _value; }
    }
}

public class EndpointTypeAttribute : Attribute, IAttribute
{

    private string _value;

    public EndpointTypeAttribute(string text)
    {
        this._value = text;
    }

    public string Value
    {
        get { return _value; }
    }
}

public class VariantType : Attribute, IAttribute
{

    private string _value;

    public VariantType(string text)
    {
        this._value = text;
    }

    public string Value
    {
        get { return _value; }
    }
}

public class TypeOfAccount : Attribute, IAttribute
{

    private string _value;

    public TypeOfAccount(string text)
    {
        this._value = text;
    }

    public string Value
    {
        get { return _value; }
    }
}

public class TypeOfEmployee : Attribute, IAttribute
{

    private string _value;

    public TypeOfEmployee(string text)
    {
        this._value = text;
    }

    public string Value
    {
        get { return _value; }
    }
}


public class ActionType : Attribute, IAttribute
{

    private string _value;

    public ActionType(string text)
    {
        this._value = text;
    }

    public string Value
    {
        get { return _value; }
    }
}