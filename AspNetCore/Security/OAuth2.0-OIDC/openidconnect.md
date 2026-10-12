# OpenID Connect

## 什么是OpenID Connect？

OpenID Connect是建立在OAuth 2.0框架（[RFC6749](https://datatracker.ietf.org/doc/html/rfc6749)和[RFC6750](https://datatracker.ietf.org/doc/html/rfc6750)）之上的规范。OAuth 2.0通过包含范围的**访问令牌**提供授权，而OpenID Connect通过引入新的令牌-身份令牌(ID token)，提供认证，该令牌包含一组新的用于身份的范围和声明。通过身份令牌，OpenID Connect提供了结构和可预测性，使得不同系统能够互操作并共享认证状态和用户个人资料信息。

OAuth 解决了代理授权的问题，但是它没有提供一个认证用户身份的标准方法。你可以这样认为：
- OAuth2.0 用于授权
- OpenID Connect 用于认证

如果你无法区分这些术语，则以下是它们之间的区别：
- 认证（Authentication）是确保通信实体是其所声称的实体。
- 授权（Authorization）是验证通信实体是否有权访问资源的过程。

换言之，认证关注的是你是谁，授权关注的是你有什么权限。

![image.png](images/202307101454784.png)

## OpenID Connect 比 OAuth2.0 有哪些变化？

OIDC 使用 OAuth 2.0 作为底层协议。在此基础上又新增了一些规范。

1. Scopes在OAuth 2.0规范中，作用域可以任意由OAuth提供者定义。虽然这种灵活性很大，但实际上使得互操作性几乎不可能。OIDC将这些作用域标准化为openid（必须）、profile、email和address。
2. 除了标准化使用的范围外，OpenID Connect 还标准化了用于 OpenID Connect 范围的Claims。正是这些标准的声明集合包含了用于认证的用户特定信息。例如，通过具体命名为 given_name 和 family_name 的声明。
3. ID Token包含有关已认证用户的一些重要信息，如用户唯一标识符（sub）、用户姓名（name）、电子邮件地址（email）等。该令牌的目的是让客户端应用程序验证用户的身份，并使用这些信息执行后续的授权和访问控制操作。
4. 除了ID令牌之外，在实现OpenID Connect时还有标准化的端点。UserInfo Endpoint：用于通过访问令牌（Access Token）获取关于已认证用户的信息，返回用户的Claims。
5. 使用“flow”来代替OAuth2的“grant”，如在OIDC中 Authorization Code Flow 其实是OAuth2.0 中的 Authorization Code Grant
6. response_type 添加Id_token返回类型。

|"response_type" value|Flow|
|:--|:--|
|code|Authorization Code Flow|
|id_token|Implicit Flow|
|id_token token|Implicit Flow|
|code id_token|Hybrid Flow|
|code token|Hybrid Flow|
|code id_token token|Hybrid Flow|

### 术语

| 简称 | 全名和相关名称 | 描述 |
| --- | --- | --- |
| Authentication | _Login_ | 验证用户身份的行为，即核实用户确实是他们所声称的人。 |
| Authorization | _role, groups, attributes, access control list, scopes_ | 授权访问特定资源的行为（针对已验证的用户或秘密持有者）。|
| OIDC | OpenID Connect | 一种标准化身份验证层，使用OAuth2进行身份验证。|
| OAuth2 | Open standard for access delegation. | 使用户或系统能够将某个资源授权给另一个资源访问数据的协议（例如，用户将某些访问权限委派给网站A，以便网站A代表用户访问网站B的数据）。|
| End User | User| 一个用户是指使用已注册的客户端访问资源的人。|
| RP & Client | Relying Party, _client, web application, web property_ | 通常，一个想要验证并最终授权访问数据的Web应用程序。客户端必须在OP中注册。客户端可以是Web应用程序，原生移动和桌面应用程序等。|
| OP & IdP | OIDC Provider, _IdP, authorization server_ | 为依赖方（RPs）提供认证和授权。一个OpenID提供者（OP）是一个实施了OpenID Connect和OAuth 2.0协议的实体，OP有时可以根据其角色称之为安全令牌服务、身份提供者（IDP）或授权服务器。|
| Scopes | _role, groups, attributes, access control list, scopes_ | 访问控制信息、用户组、角色、属性等，由依赖方（RP）用于向用户授予特定的授权/访问权限。 |
| SSO | Single Sign On | 一个OIDC提供者（OP）和一组依赖方（RPs），为用户提供独特的登录面板，并统一处理用户的会话信息。 |
| Identity Token | - |身份令牌代表了身份验证过程的结果。它至少包含用户的标识符（称为子主题声明），以及关于用户是如何和何时进行认证的信息。它还可以包含其他身份数据。 |
| Access token | - | 在OAuth2中定义，这个令牌提供对请求授权服务器中Scopes值所定义的特定用户资源的访问。 |
| Refresh token| - |来自OAuth2规范，这个令牌通常具有长时间的生命周期，可以用来获取新的访问令牌。|



### 流程图

图例地址：[Final: OpenID Connect Core 1.0 incorporating errata set 1](https://openid.net/specs/openid-connect-core-1_0.html#Overview)

```
+--------+                                   +--------+
|        |                                   |        |
|        |---------(1) AuthN Request-------->|        |
|        |                                   |        |
|        |  +--------+                       |        |
|        |  |        |                       |        |
|        |  |  End-  |<--(2) AuthN & AuthZ-->|        |
|        |  |  User  |                       |        |
|   RP   |  |        |                       |   OP   |
|        |  +--------+                       |        |
|        |                                   |        |
|        |<--------(3) AuthN Response--------|        |
|        |                                   |        |
|        |---------(4) UserInfo Request----->|        |
|        |                                   |        |
|        |<--------(5) UserInfo Response-----|        |
|        |                                   |        |
+--------+                                   +--------+
```

图例2：
![image.png](images/202307111143773.png)

### 概要流程

1. 用户向客户端（我们的应用）发起登录请求。
2. 客户端（我们的应用）将用户重定向到 OpenID 提供商(OpenID Provider, OP)的认证端点。该服务器通常从身份提供者（IdP）获取用户凭据和属性信息的数据库中获取用户信息。
3. 用户在 OP 端点上进行认证，提供用户名和密码等凭证。
4. OP验证请求参数的正确性，并返回访问令牌(Access Token)和可选的身份令牌(ID Token)。ID Token 向Web应用程序（RP）提供关于用户何时进行身份验证、各种属性以及用户会话可以信任多长时间的信息。
5. 客户端应用使用访问令牌向资源服务器发送请求，获取用户资源。

### 几种授权流程

1. 授权码流程（ 最常用的流程），用于传统的Web应用程序以及原生/移动应用程序。包括将初始浏览器重定向到/从OP进行用户身份验证和同意，然后进行第二个后台请求以检索ID令牌。此流程提供最佳的安全性，因为令牌不会暴露给浏览器，客户端也可以进行身份验证。
2.  隐式流程 （ 用于没有后端的浏览器（JavaScript）应用程序）ID令牌直接与OP的重定向响应一起接收。此处不需要后台请求。responsetype=token id_token/id_token
3. 混合流程 （ 很少使用），允许应用程序前端和后端分别接收令牌。基本上是代码和隐式流程的组合。responsetype=code id_token/code token/code id_token token

|Property|Authorization Code Flow|Implicit Flow|Hybrid Flow|
|:--|:--|:--|:--|
|access token和id token都通过Authorization endpoint返回|no|yes|no|
|两个token都通过token end point 返回|yes|no|no|
|用户使用的端(浏览器或者手机）无法查看token|yes|no|no|
|Client can be authenticated|yes|no|yes|
|支持刷新token|yes|no|yes|
|不需要后端参与|no|yes|no|

##  授权码流程

[授权码流程]([Final: OpenID Connect Core 1.0 incorporating errata set 1](https://openid.net/specs/openid-connect-core-1_0.html#CodeFlowSteps)) 规范地址。

![image.png](images/202307111159809.png)


1. 用户向客户端应用发起登录请求。
2. 客户端应用将用户重定向到 OpenID 提供商(identity provider, IdP)的认证端点。
3. 用户在 IdP 端点上进行认证，提供用户名和密码等凭证。
4. IdP 对用户进行认证，并生成授权码(Authorization Code)。
5. IdP 将授权码发送回客户端应用的重定向 URI。
6. 客户端应用使用授权码向 IdP 的 Token 端点发送请求，包括客户端 ID、客户端密钥、授权码和重定向 URI。
7. IdP 验证请求参数的正确性，并返回访问令牌(Access Token)和可选的身份令牌(ID Token)。
8. 客户端应用使用访问令牌向资源服务器发送请求，获取用户资源。
9. 资源服务器验证访问令牌的有效性，如果有效，则返回用户资源的响应给客户端应用。

### 授权端点

授权端点是 OpenID 提供商（Identity Provider, IdP）的认证服务端点，用于处理用户的身份验证和授权请求。授权端点用于执行以下操作：
1. 用户向客户端应用发起登录请求，客户端应用将用户重定向至授权端点。
2. 用户在授权端点上进行身份验证。
3. 授权端点根据身份验证结果生成并返回授权码。

### 授权请求

在向授权服务端发起用户登录和授权告知的请求时，定义一个名叫`openid`的授权范围。在告知授权服务器需要使用 OpenID Connect 时，`openid`是必须存在的scope，如果不在会认为一个OAuth2.0请求。

```
https://server.example.com/authorize?
    response_type=code
    &scope=openid%20profile%20email
    &client_id=s6BhdRkqt3
    &state=af0ifjsldkj
    &redirect_uri=https%3A%2F%2Fclient.example.org%2Fcb
```

该请求的返回结果是客户端可以用来交换访问令牌和 ID 令牌的授权码。如果 OAuth 流程是隐式的，那么授权服务端将直接返回访问令牌和 ID 令牌。

[ID 令牌]([Final: OpenID Connect Core 1.0 incorporating errata set 1](https://openid.net/specs/openid-connect-core-1_0.html#IDToken))是 JWT，或者又称 JSON Web Token。JWT 是一个编码令牌，它由三部分组成：头部，有效负载和签名。在获得了 ID 令牌后，客户端可以将其解码，并且得到被编码在有效负载中的用户信息，如以下例子所示：

```json
{
   "iss": "https://server.example.com",
   "sub": "24400320",
   "aud": "s6BhdRkqt3",
   "nonce": "n-0S6_WzA2Mj",
   "exp": 1311281970,
   "iat": 1311280970,
   "auth_time": 1311280969,
   "acr": "urn:mace:incommon:iap:silver"
  }
```

ID 令牌的有效负载包括了一些被称作声明的域。基本的必填声明有：

- iss：令牌发布者
- sub：用户的唯一标识符
- email：用户的邮箱
- aud：Audience(s) 标识ID Token的受众。
- iat：用 Unix 时间表示的令牌发布时间
- exp：Unix 时间表示的令牌到期时间
- nonce：RP发送请求的时候提供的随机字符串，用来减缓重放攻击。
- amr： Authentication Methods References：可选。表示一组认证方法。
- acr： Authentication Context Class Reference：可选。表示一个认证上下文引用值，可以用来标识认证上下文类。

如果客户端需要更多的用户信息，客户端可以指定标准的 OpenID Connect 范围，来告知授权服务端将所需信息包括在 ID 令牌的有效负载中。这些范围包括个人主页（profile）、邮箱（email）、地址（address）和电话（phone）。

### 授权响应

OpenID Connect 的认证响应是在用户登录成功后，OpenID 提供商（IdP）将认证授权码（Authorization Code）通过重定向（Redirect）方式返回给客户端应用的认证端点的响应。

```
HTTP/1.1 302 Found
  Location: https://client.example.org/cb?
    code=SplxlOBeZQQYbYS6WxSbIA
    &state=af0ifjsldkj
```

认证响应通常包含以下参数：

1. 授权码（authorization_code）：这是一个短暂的代码，用于向客户端应用证明用户身份验证已成功。客户端应用将使用授权码来获取访问令牌。
2. 状态码（state）：这是客户端应用在发起认证请求时传递给认证端点的参数，用于防止跨站请求伪造攻击（CSRF）。认证端点会将该参数原样返回给客户端应用，以便客户端可以验证请求的合法性。


### Token端点

OpenID Connect Token 端点的主要作用是为客户端应用颁发访问令牌和身份令牌或者可能的刷新令牌。以下是 Token 端点的主要作用：

1. 颁发访问令牌：Token 端点允许客户端应用使用授权码（Authorization Code）或其他身份验证令牌来请求访问令牌。访问令牌用于客户端应用向受保护的资源服务器发送请求，并获取受保护资源。
2. 颁发身份令牌：Token 端点有时也会颁发身份令牌，例如 OpenID Connect 中的 ID 令牌。身份令牌包含有关用户身份和其他相关信息的声明，用于客户端应用进行身份验证和处理用户信息。
3. 刷新访问令牌：Token 端点允许客户端应用使用刷新令牌来请求新的访问令牌。刷新令牌通常在访问令牌过期后使用，以避免用户重新进行身份验证。刷新令牌可用于获取新的访问令牌和更新的刷新令牌。

### Token请求

一个客户端 [Token Request](https://openid.net/specs/openid-connect-core-1_0.html#TokenRequest)通过将之前获取 Authorization Code 带入请求参数中。 如果客户端类型是机密的，或者客户端被分配了客户端凭证（或被指定了其他认证要求），那么必须在获取Token时候对客户端进行认证。

```
 POST /token HTTP/1.1
  Host: server.example.com
  Content-Type: application/x-www-form-urlencoded
  Authorization: Basic czZCaGRSa3F0MzpnWDFmQmF0M2JW

  grant_type=authorization_code&code=SplxlOBeZQQYbYS6WxSbIA
    &redirect_uri=https%3A%2F%2Fclient.example.org%2Fcb
```


### Token响应

在从客户端接收并验证有效授权的令牌请求后，授权服务器返回一个成功的响应，其中包括一个ID令牌和一个访问令牌。

除了OAuth2.0规范定义的响应参数，id_token 是必须返回的。

```
HTTP/1.1 200 OK
  Content-Type: application/json
  Cache-Control: no-store
  Pragma: no-cache

  {
   "access_token": "SlAV32hkKG",
   "token_type": "Bearer",
   "refresh_token": "8xLOxBtZp8",
   "expires_in": 3600,
   "id_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6IjFlOWdkazcifQ.ewogImlzc
     yI6ICJodHRwOi8vc2VydmVyLmV4YW1wbGUuY29tIiwKICJzdWIiOiAiMjQ4Mjg5
     NzYxMDAxIiwKICJhdWQiOiAiczZCaGRSa3F0MyIsCiAibm9uY2UiOiAibi0wUzZ
     fV3pBMk1qIiwKICJleHAiOiAxMzExMjgxOTcwLAogImlhdCI6IDEzMTEyODA5Nz
     AKfQ.ggW8hZ1EuVLuxNuuIJKX_V8a_OMXzR0EHR9R6jgdqrOOF4daGU96Sr_P6q
     Jp6IcmD3HP99Obi1PRs-cwh3LO-p146waJ8IhehcwL7F09JdijmBqkvPeB2T9CJ
     NqeGpe-gccMg4vfKjkM8FcGvnzZUN4_KSP0aAp1tOJ1zZwgjxqGByKHiOtX7Tpd
     QyHE5lcMiKPXfEIQILVq0pc_E2DzL7emopWoaoZTF_m0_N0YzFC6g6EJbOEoRoS
     K5hoDalrcvRYLSrQAZZKflyuVCyixEoV9GfNQC3_osjzw2PAithfubEEBLuVVk4
     XUVrWOLrLl0nx7RkKU8NXNHq-rvKMzqg"
  }
```


## 隐式流程

[OpenID Connect 隐式流程 ]([Final: OpenID Connect Core 1.0 incorporating errata set 1](https://openid.net/specs/openid-connect-core-1_0.html#ImplicitFlowAuth))是一种用于实现客户端应用获取访问令牌和身份令牌的授权流程，适用于无法在后端进行密钥保护和处理的纯前端应用程序。在使用隐式流程时，所有的令牌都从授权端点返回；不使用令牌端点。

1. 客户端应用重定向至 OpenID 提供商（IdP）的授权端点，并包含必要的参数，如客户端ID、重定向URI、响应类型（通常是 "token id_token"）和可选的权限范围等。
2. 用户在 IdP 的登录界面上进行身份验证，并授权客户端应用访问其受保护的资源。
3. IdP 验证用户身份后，将生成访问令牌和身份令牌，并将其作为 URI 的片段参数附加到重定向URI上。例如，将令牌作为 "#access_token=xxxx&id_token=yyyy" 附加到重定向URI 上。
4. 客户端应用从重定向URI 中提取令牌，例如使用 JavaScript 从当前 URL 中获取 URI 片段参数。
5. 客户端应用验证令牌的有效性，包括验证访问令牌的签名和过期时间，以及验证身份令牌的签名和有效性。
6. 客户端应用可以使用访问令牌来访问受保护的资源服务器，并使用身份令牌来获取用户的身份信息。

### 授权端点
在使用隐式流时，授权终端的使用方式与授权码流相同。

### 授权请求

OpenID Connect 隐式流程的请求参数包括：
1. response_type（必需）：指定要求的响应类型。在隐式流程中，使用 "token id_token" 表示要求获取访问令牌和身份令牌。
2. client_id（必需）：标识客户端应用的唯一标识符。
3. redirect_uri（必需）：指定 OpenID 提供商授权后重定向回客户端应用的 URI。
4.  nonce（必需）：用于隐式流程的安全性，用于保证身份令牌的安全性。客户端应用生成的随机值，并在验证身份令牌时进行校验。
5. state（推荐）：用于客户端应用在请求和响应之间维持状态的参数。可以用于防止跨站请求伪造（CSRF）攻击。

```
 GET /authorize?
    response_type=id_token%20token
    &client_id=s6BhdRkqt3
    &redirect_uri=https%3A%2F%2Fclient.example.org%2Fcb
    &scope=openid%20profile
    &state=af0ifjsldkj
    &nonce=n-0S6_WzA2Mj HTTP/1.1
  Host: server.example.com
```

### 授权响应

当使用隐式流时，验证响应的方式与授权码流相同。额外返回参数说明如下：

1. access_token：访问令牌，用于客户端应用向受保护的资源服务器发送请求并获取受保护资源。
2. token_type：令牌类型，通常为 "bearer" 表示访问令牌类型。    
3. id_token：身份令牌，包含有关用户身份和其他相关信息的声明，用于客户端应用进行身份验证和处理用户信息。
4. expires_in：访问令牌的过期时间（以秒为单位）。客户端应用需要在令牌过期之前使用这个时间内的访问令牌。
5. state：原始请求中传递的参数值，用于客户端应用在请求和响应之间维持状态。

```
 HTTP/1.1 302 Found
  Location: https://client.example.org/cb#
    access_token=SlAV32hkKG
    &token_type=bearer
    &id_token=eyJ0 ... NiJ9.eyJ1c ... I6IjIifX0.DeWt4Qu ... ZXso
    &expires_in=3600
    &state=af0ifjsldkj
```


## Standard Claims

OpenID Connect规定了一组[标准声明](https://openid.net/specs/openid-connect-core-1_0.html#StandardClaims)，或者称为用户属性。它们旨在在请求时向客户端提供用户的详细信息，例如电子邮件、姓名和照片。

| 类别 |关联的 claims|
|---|---|
|email|email, email_verified|
|phone|phone_number, phone_number_verified|
|profile|name, family_name, given_name, middle_name, nickname, preferred_username, profile, picture, website, gender, birthdate, zoneinfo, locale, updated_at|
|address|address|