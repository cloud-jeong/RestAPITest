using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;
using System = System;

//[Route("api/workspace")]
//[ApiController]
public partial class WorkspaceController
{
    private string _baseUrl = "http://localhost/api/v2.0";
    private System.Net.Http.HttpClient _httpClient;
    private System.Lazy<Newtonsoft.Json.JsonSerializerSettings> _settings;

    public WorkspaceController(System.Net.Http.HttpClient httpClient)
    {
        _httpClient = httpClient;
        _settings = new System.Lazy<Newtonsoft.Json.JsonSerializerSettings>(CreateSerializerSettings);
    }

    private Newtonsoft.Json.JsonSerializerSettings CreateSerializerSettings()
    {
        var settings = new Newtonsoft.Json.JsonSerializerSettings();
        UpdateJsonSerializerSettings(settings);
        return settings;
    }

    public string BaseUrl
    {
        get { return _baseUrl; }
        set { _baseUrl = value; }
    }

    protected Newtonsoft.Json.JsonSerializerSettings JsonSerializerSettings { get { return _settings.Value; } }
    
    partial void UpdateJsonSerializerSettings(Newtonsoft.Json.JsonSerializerSettings settings);

    partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url);
    partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, System.Text.StringBuilder urlBuilder);
    partial void ProcessResponse(System.Net.Http.HttpClient client, System.Net.Http.HttpResponseMessage response);

    public bool ReadResponseAsString { get; set; }

    protected struct ObjectResponseResult<T>
    {
        public ObjectResponseResult(T responseObject, string responseText)
        {
            this.Object = responseObject;
            this.Text = responseText;
        }

        public T Object { get; }

        public string Text { get; }
    }

    protected virtual async System.Threading.Tasks.Task<ObjectResponseResult<T>> ReadObjectResponseAsync<T>(System.Net.Http.HttpResponseMessage response, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, System.Threading.CancellationToken cancellationToken)
    {
        if (response == null || response.Content == null)
        {
            return new ObjectResponseResult<T>(default(T), string.Empty);
        }

        if (ReadResponseAsString)
        {
            var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            try
            {
                var typedBody = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseText, JsonSerializerSettings);
                return new ObjectResponseResult<T>(typedBody, responseText);
            }
            catch (Newtonsoft.Json.JsonException exception)
            {
                var message = "Could not deserialize the response body string as " + typeof(T).FullName + ".";
                throw new ApiException(message, (int)response.StatusCode, responseText, headers, exception);
            }
        }
        else
        {
            try
            {
                using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                using (var streamReader = new System.IO.StreamReader(responseStream))
                using (var jsonTextReader = new Newtonsoft.Json.JsonTextReader(streamReader))
                {
                    var serializer = Newtonsoft.Json.JsonSerializer.Create(JsonSerializerSettings);
                    var typedBody = serializer.Deserialize<T>(jsonTextReader);
                    return new ObjectResponseResult<T>(typedBody, string.Empty);
                }
            }
            catch (Newtonsoft.Json.JsonException exception)
            {
                var message = "Could not deserialize the response body stream as " + typeof(T).FullName + ".";
                throw new ApiException(message, (int)response.StatusCode, string.Empty, headers, exception);
            }
        }
    }

    private string ConvertToString(object value, System.Globalization.CultureInfo cultureInfo)
    {
        if (value == null)
        {
            return "";
        }

        if (value is System.Enum)
        {
            var name = System.Enum.GetName(value.GetType(), value);
            if (name != null)
            {
                var field = System.Reflection.IntrospectionExtensions.GetTypeInfo(value.GetType()).GetDeclaredField(name);
                if (field != null)
                {
                    var attribute = System.Reflection.CustomAttributeExtensions.GetCustomAttribute(field, typeof(System.Runtime.Serialization.EnumMemberAttribute))
                        as System.Runtime.Serialization.EnumMemberAttribute;
                    if (attribute != null)
                    {
                        return attribute.Value != null ? attribute.Value : name;
                    }
                }

                var converted = System.Convert.ToString(System.Convert.ChangeType(value, System.Enum.GetUnderlyingType(value.GetType()), cultureInfo));
                return converted == null ? string.Empty : converted;
            }
        }
        else if (value is bool)
        {
            return System.Convert.ToString((bool)value, cultureInfo).ToLowerInvariant();
        }
        else if (value is byte[])
        {
            return System.Convert.ToBase64String((byte[])value);
        }
        else if (value.GetType().IsArray)
        {
            var array = System.Linq.Enumerable.OfType<object>((System.Array)value);
            return string.Join(",", System.Linq.Enumerable.Select(array, o => ConvertToString(o, cultureInfo)));
        }

        var result = System.Convert.ToString(value, cultureInfo);
        return result == null ? "" : result;
    }


    public virtual Task CreateProjectAsync(string x_Request_Id, bool? x_Resource_Name_In_Location, ProjectReq project)
    {
        return CreateProjectAsync(x_Request_Id, x_Resource_Name_In_Location, project, System.Threading.CancellationToken.None);
    }

    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <summary>
    /// Create a new project.
    /// </summary>
    /// <param name="x_Request_Id">An unique ID for the request</param>
    /// <param name="x_Resource_Name_In_Location">The flag to indicate whether to return the name of the resource in Location. When X-Resource-Name-In-Location is true, the Location will return the name of the resource.</param>
    /// <param name="project">New created project.</param>
    /// <returns>Created</returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    public virtual async Task CreateProjectAsync(string x_Request_Id, bool? x_Resource_Name_In_Location, ProjectReq project, System.Threading.CancellationToken cancellationToken)
    {
        if (project == null)
            throw new ArgumentNullException("project");

        var urlBuilder_ = new System.Text.StringBuilder();
        urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/projects");

        var client_ = _httpClient;
        var disposeClient_ = false;
        try
        {
            using var request_ = new HttpRequestMessage();

            if (x_Request_Id != null)
                request_.Headers.TryAddWithoutValidation("X-Request-Id", ConvertToString(x_Request_Id, System.Globalization.CultureInfo.InvariantCulture));

            if (x_Resource_Name_In_Location != null)
                request_.Headers.TryAddWithoutValidation("X-Resource-Name-In-Location", ConvertToString(x_Resource_Name_In_Location, System.Globalization.CultureInfo.InvariantCulture));


            var content_ = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(project, _settings.Value));
            content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");

            request_.Content = content_;
            request_.Method = new HttpMethod("POST");

            PrepareRequest(client_, request_, urlBuilder_);

            var url_ = urlBuilder_.ToString();
            request_.RequestUri = new Uri(url_, System.UriKind.RelativeOrAbsolute);

            PrepareRequest(client_, request_, url_);

            var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            var disposeResponse_ = true;
            try
            {
                var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                if (response_.Content != null && response_.Content.Headers != null)
                {
                    foreach (var item_ in response_.Content.Headers)
                        headers_[item_.Key] = item_.Value;
                }

                ProcessResponse(client_, response_);

                var status_ = (int)response_.StatusCode;
                if (status_ == 201)
                {
                    return;
                }
                else
                if (status_ == 400)
                {
                    var objectResponse_ = await ReadObjectResponseAsync<Errors>(response_, headers_, cancellationToken).ConfigureAwait(false);
                    if (objectResponse_.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                    }
                    throw new ApiException<Errors>("Bad request", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                }
                else
                if (status_ == 401)
                {
                    var objectResponse_ = await ReadObjectResponseAsync<Errors>(response_, headers_, cancellationToken).ConfigureAwait(false);
                    if (objectResponse_.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                    }
                    throw new ApiException<Errors>("Unauthorized", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                }
                else
                if (status_ == 409)
                {
                    var objectResponse_ = await ReadObjectResponseAsync<Errors>(response_, headers_, cancellationToken).ConfigureAwait(false);
                    if (objectResponse_.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                    }
                    throw new ApiException<Errors>("Conflict", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                }
                else
                if (status_ == 500)
                {
                    var objectResponse_ = await ReadObjectResponseAsync<Errors>(response_, headers_, cancellationToken).ConfigureAwait(false);
                    if (objectResponse_.Object == null)
                    {
                        throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                    }
                    throw new ApiException<Errors>("Internal server error", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                }
                else
                {
                    var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                }
            }
            finally
            {
                if (disposeResponse_)
                    response_.Dispose();
            }
        }
        finally
        {
            if (disposeClient_)
                client_.Dispose();
        }
    }
}

public partial class ProjectMetadata
{
    /// <summary>
    /// The public status of the project. The valid values are "true", "false".
    /// </summary>
    [Newtonsoft.Json.JsonProperty("public", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string Public { get; set; }

    /// <summary>
    /// Whether content trust is enabled or not. If it is enabled, user can't pull unsigned images from this project. The valid values are "true", "false".
    /// </summary>
    [Newtonsoft.Json.JsonProperty("enable_content_trust", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string Enable_content_trust { get; set; }

    /// <summary>
    /// Whether cosign content trust is enabled or not. If it is enabled, user can't pull images without cosign signature from this project. The valid values are "true", "false".
    /// </summary>
    [Newtonsoft.Json.JsonProperty("enable_content_trust_cosign", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string Enable_content_trust_cosign { get; set; }

    /// <summary>
    /// Whether prevent the vulnerable images from running. The valid values are "true", "false".
    /// </summary>
    [Newtonsoft.Json.JsonProperty("prevent_vul", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string Prevent_vul { get; set; }

    /// <summary>
    /// If the vulnerability is high than severity defined here, the images can't be pulled. The valid values are "none", "low", "medium", "high", "critical".
    /// </summary>
    [Newtonsoft.Json.JsonProperty("severity", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string Severity { get; set; }

    /// <summary>
    /// Whether scan images automatically when pushing. The valid values are "true", "false".
    /// </summary>
    [Newtonsoft.Json.JsonProperty("auto_scan", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string Auto_scan { get; set; }

    /// <summary>
    /// Whether this project reuse the system level CVE allowlist as the allowlist of its own.  The valid values are "true", "false". If it is set to "true" the actual allowlist associate with this project, if any, will be ignored.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("reuse_sys_cve_allowlist", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string Reuse_sys_cve_allowlist { get; set; }

    /// <summary>
    /// The ID of the tag retention policy for the project
    /// </summary>
    [Newtonsoft.Json.JsonProperty("retention_id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string Retention_id { get; set; }

}

public partial class CVEAllowlistItem
{
    /// <summary>
    /// The ID of the CVE, such as "CVE-2019-10164"
    /// </summary>
    [Newtonsoft.Json.JsonProperty("cve_id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string Cve_id { get; set; }

}

public partial class CVEAllowlist
{
    /// <summary>
    /// ID of the allowlist
    /// </summary>
    [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int? Id { get; set; }

    /// <summary>
    /// ID of the project which the allowlist belongs to.  For system level allowlist this attribute is zero.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("project_id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int? Project_id { get; set; }

    /// <summary>
    /// the time for expiration of the allowlist, in the form of seconds since epoch.  This is an optional attribute, if it's not set the CVE allowlist does not expire.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("expires_at", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public int? Expires_at { get; set; }

    [Newtonsoft.Json.JsonProperty("items", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public System.Collections.Generic.ICollection<CVEAllowlistItem> Items { get; set; }

    /// <summary>
    /// The creation time of the allowlist.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("creation_time", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public System.DateTimeOffset? Creation_time { get; set; }

    /// <summary>
    /// The update time of the allowlist.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("update_time", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public System.DateTimeOffset? Update_time { get; set; }

}

public partial class ProjectReq
{
    /// <summary>
    /// The name of the project.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("project_name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    [System.ComponentModel.DataAnnotations.StringLength(255)]
    public string Project_name { get; set; }

    /// <summary>
    /// deprecated, reserved for project creation in replication
    /// </summary>
    [Newtonsoft.Json.JsonProperty("public", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public bool? Public { get; set; }

    /// <summary>
    /// The metadata of the project.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("metadata", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public ProjectMetadata Metadata { get; set; }

    /// <summary>
    /// The CVE allowlist of the project.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("cve_allowlist", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public CVEAllowlist Cve_allowlist { get; set; }

    /// <summary>
    /// The storage quota of the project.
    /// </summary>
    [Newtonsoft.Json.JsonProperty("storage_limit", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public long? Storage_limit { get; set; }

    /// <summary>
    /// The ID of referenced registry when creating the proxy cache project
    /// </summary>
    [Newtonsoft.Json.JsonProperty("registry_id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public long? Registry_id { get; set; }

}

public partial class ApiException : System.Exception
{
    public int StatusCode { get; private set; }

    public string Response { get; private set; }

    public System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> Headers { get; private set; }

    public ApiException(string message, int statusCode, string response, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, System.Exception innerException)
        : base(message + "\n\nStatus: " + statusCode + "\nResponse: \n" + ((response == null) ? "(null)" : response.Substring(0, response.Length >= 512 ? 512 : response.Length)), innerException)
    {
        StatusCode = statusCode;
        Response = response;
        Headers = headers;
    }

    public override string ToString()
    {
        return string.Format("HTTP Response: \n\n{0}\n\n{1}", Response, base.ToString());
    }
}

[System.CodeDom.Compiler.GeneratedCode("NSwag", "13.16.1.0 (NJsonSchema v10.7.2.0 (Newtonsoft.Json v13.0.0.0))")]
public partial class ApiException<TResult> : ApiException
{
    public TResult Result { get; private set; }

    public ApiException(string message, int statusCode, string response, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, TResult result, System.Exception innerException)
        : base(message, statusCode, response, headers, innerException)
    {
        Result = result;
    }
}

public partial class Errors
{
    [Newtonsoft.Json.JsonProperty("errors", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public System.Collections.Generic.ICollection<Error> Errors1 { get; set; }

}

public partial class Error
{
    /// <summary>
    /// The error code
    /// </summary>
    [Newtonsoft.Json.JsonProperty("code", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string Code { get; set; }

    /// <summary>
    /// The error message
    /// </summary>
    [Newtonsoft.Json.JsonProperty("message", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string Message { get; set; }

}