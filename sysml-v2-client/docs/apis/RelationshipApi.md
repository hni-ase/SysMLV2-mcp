# Org.OpenAPITools.Api.RelationshipApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**GetRelationshipsByProjectCommitRelatedElement**](RelationshipApi.md#getrelationshipsbyprojectcommitrelatedelement) | **GET** /projects/{projectId}/commits/{commitId}/elements/{relatedElementId}/relationships | Get relationships by project, commit, and related element |

<a id="getrelationshipsbyprojectcommitrelatedelement"></a>
# **GetRelationshipsByProjectCommitRelatedElement**
> List&lt;Relationship&gt; GetRelationshipsByProjectCommitRelatedElement (Guid projectId, Guid commitId, Guid relatedElementId, string direction = null, string pageAfter = null, string pageBefore = null, int pageSize = null)

Get relationships by project, commit, and related element


### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **projectId** | **Guid** | ID of the project |  |
| **commitId** | **Guid** | ID of the commit |  |
| **relatedElementId** | **Guid** | ID of the related element |  |
| **direction** | **string** | Filter for relationships that are incoming (in), outgoing (out), or both relative to the related element | [optional] [default to both] |
| **pageAfter** | **string** | Page after | [optional]  |
| **pageBefore** | **string** | Page before | [optional]  |
| **pageSize** | **int** | Page size | [optional]  |

### Return type

[**List&lt;Relationship&gt;**](Relationship.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json, application/ld+json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Ok |  -  |
| **415** | The requested content type is not acceptable. |  -  |
| **500** | Internal server error. |  -  |
| **0** | Unexpected response. |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

