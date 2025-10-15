# Org.OpenAPITools.Api.QueryApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**DeleteQueryByProjectAndId**](QueryApi.md#deletequerybyprojectandid) | **DELETE** /projects/{projectId}/queries/{queryId} | Delete query by project and ID |
| [**GetQueriesByProject**](QueryApi.md#getqueriesbyproject) | **GET** /projects/{projectId}/queries | Get queries by project |
| [**GetQueryByProjectAndId**](QueryApi.md#getquerybyprojectandid) | **GET** /projects/{projectId}/queries/{queryId} | Get query by project and ID |
| [**GetQueryResultsByProjectIdQuery**](QueryApi.md#getqueryresultsbyprojectidquery) | **GET** /projects/{projectId}/query-results | Get query results by project and query definition |
| [**GetQueryResultsByProjectIdQueryId**](QueryApi.md#getqueryresultsbyprojectidqueryid) | **GET** /projects/{projectId}/queries/{queryId}/results | Get query results by project and query |
| [**GetQueryResultsByProjectIdQueryPost**](QueryApi.md#getqueryresultsbyprojectidquerypost) | **POST** /projects/{projectId}/query-results | Get query results by project and query definition via POST |
| [**PostQueryByProject**](QueryApi.md#postquerybyproject) | **POST** /projects/{projectId}/queries | Create query by project |

<a id="deletequerybyprojectandid"></a>
# **DeleteQueryByProjectAndId**
> Query DeleteQueryByProjectAndId (Guid projectId, Guid queryId)

Delete query by project and ID


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | ID of the project |  |
| **queryId** | **Guid** | ID of the query |  |

### Return type

[**Query**](Query.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Ok |  -  |
| **404** | Not found. |  -  |
| **415** | The requested content type is not acceptable. |  -  |
| **500** | Internal server error. |  -  |
| **0** | Unexpected response. |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getqueriesbyproject"></a>
# **GetQueriesByProject**
> List&lt;Query&gt; GetQueriesByProject (Guid projectId, string pageAfter = null, string pageBefore = null, int pageSize = null)

Get queries by project


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | ID of the project |  |
| **pageAfter** | **string** | Page after | [optional]  |
| **pageBefore** | **string** | Page before | [optional]  |
| **pageSize** | **int** | Page size | [optional]  |

### Return type

[**List&lt;Query&gt;**](Query.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Ok |  -  |
| **404** | Not found. |  -  |
| **415** | The requested content type is not acceptable. |  -  |
| **500** | Internal server error. |  -  |
| **0** | Unexpected response. |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getquerybyprojectandid"></a>
# **GetQueryByProjectAndId**
> Query GetQueryByProjectAndId (Guid projectId, Guid queryId)

Get query by project and ID


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | ID of the project |  |
| **queryId** | **Guid** | ID of the query |  |

### Return type

[**Query**](Query.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Ok |  -  |
| **404** | Not found. |  -  |
| **415** | The requested content type is not acceptable. |  -  |
| **500** | Internal server error. |  -  |
| **0** | Unexpected response. |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getqueryresultsbyprojectidquery"></a>
# **GetQueryResultsByProjectIdQuery**
> List&lt;Element&gt; GetQueryResultsByProjectIdQuery (Guid projectId, Query body, Guid commitId = null)

Get query results by project and query definition


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | ID of the project |  |
| **body** | [**Query**](Query.md) |  |  |
| **commitId** | **Guid** | ID of the commit - defaults to head of project | [optional]  |

### Return type

[**List&lt;Element&gt;**](Element.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json, application/ld+json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Ok |  -  |
| **404** | Not found. |  -  |
| **415** | The requested content type is not acceptable. |  -  |
| **500** | Internal server error. |  -  |
| **0** | Unexpected response. |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getqueryresultsbyprojectidqueryid"></a>
# **GetQueryResultsByProjectIdQueryId**
> List&lt;Element&gt; GetQueryResultsByProjectIdQueryId (Guid projectId, Guid queryId, Guid commitId = null)

Get query results by project and query


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | ID of the project |  |
| **queryId** | **Guid** | ID of the query |  |
| **commitId** | **Guid** | ID of the commit - defaults to head of project | [optional]  |

### Return type

[**List&lt;Element&gt;**](Element.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json, application/ld+json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Ok |  -  |
| **404** | Not found. |  -  |
| **415** | The requested content type is not acceptable. |  -  |
| **500** | Internal server error. |  -  |
| **0** | Unexpected response. |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getqueryresultsbyprojectidquerypost"></a>
# **GetQueryResultsByProjectIdQueryPost**
> List&lt;Element&gt; GetQueryResultsByProjectIdQueryPost (Guid projectId, Query body, Guid commitId = null)

Get query results by project and query definition via POST

For compatibility with clients that don't support GET requests with a body


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | ID of the project |  |
| **body** | [**Query**](Query.md) |  |  |
| **commitId** | **Guid** | ID of the commit - defaults to head of project | [optional]  |

### Return type

[**List&lt;Element&gt;**](Element.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json, application/ld+json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Ok |  -  |
| **404** | Not found. |  -  |
| **415** | The requested content type is not acceptable. |  -  |
| **500** | Internal server error. |  -  |
| **0** | Unexpected response. |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="postquerybyproject"></a>
# **PostQueryByProject**
> Query PostQueryByProject (Guid projectId, Query body)

Create query by project


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | ID of the project |  |
| **body** | [**Query**](Query.md) |  |  |

### Return type

[**Query**](Query.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **201** | Created |  -  |
| **415** | The requested content type is not acceptable. |  -  |
| **500** | Internal server error. |  -  |
| **0** | Unexpected response. |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

