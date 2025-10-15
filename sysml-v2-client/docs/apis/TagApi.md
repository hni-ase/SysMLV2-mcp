# Org.OpenAPITools.Api.TagApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**DeleteTagByProjectAndId**](TagApi.md#deletetagbyprojectandid) | **DELETE** /projects/{projectId}/tags/{tagId} | Delete tag by project and ID |
| [**GetTagByProjectAndId**](TagApi.md#gettagbyprojectandid) | **GET** /projects/{projectId}/tags/{tagId} | Get tag by project and ID |
| [**GetTagsByProject**](TagApi.md#gettagsbyproject) | **GET** /projects/{projectId}/tags | Get tags by project |
| [**PostTagByProject**](TagApi.md#posttagbyproject) | **POST** /projects/{projectId}/tags | Create tag by project |

<a id="deletetagbyprojectandid"></a>
# **DeleteTagByProjectAndId**
> Tag DeleteTagByProjectAndId (Guid projectId, Guid tagId)

Delete tag by project and ID


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | ID of the project |  |
| **tagId** | **Guid** | ID of the tag |  |

### Return type

[**Tag**](Tag.md)

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

<a id="gettagbyprojectandid"></a>
# **GetTagByProjectAndId**
> Tag GetTagByProjectAndId (Guid projectId, Guid tagId)

Get tag by project and ID


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | ID of the project |  |
| **tagId** | **Guid** | ID of the tag |  |

### Return type

[**Tag**](Tag.md)

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

<a id="gettagsbyproject"></a>
# **GetTagsByProject**
> List&lt;Tag&gt; GetTagsByProject (Guid projectId, string pageAfter = null, string pageBefore = null, int pageSize = null)

Get tags by project


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | ID of the project |  |
| **pageAfter** | **string** | Page after | [optional]  |
| **pageBefore** | **string** | Page before | [optional]  |
| **pageSize** | **int** | Page size | [optional]  |

### Return type

[**List&lt;Tag&gt;**](Tag.md)

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

<a id="posttagbyproject"></a>
# **PostTagByProject**
> Branch PostTagByProject (Guid projectId, Tag body)

Create tag by project


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | ID of the project |  |
| **body** | [**Tag**](Tag.md) |  |  |

### Return type

[**Branch**](Branch.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json, application/ld+json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **201** | Created |  -  |
| **415** | The requested content type is not acceptable. |  -  |
| **500** | Internal server error. |  -  |
| **0** | Unexpected response. |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

