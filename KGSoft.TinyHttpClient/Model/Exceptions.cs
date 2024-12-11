using System;

namespace KGSoft.TinyHttpClient.Model;

public class MissingHttpMethodException : Exception { }
public class MissingUriException : Exception { }
public class ConflictingContentException : Exception
{
    public ConflictingContentException() : base("Form encoded parameters and Content have been defined. Please define one or the other, not both") { }
}
