# OAuth2.0

## 什么是OAuth2.0

OAuth2.0 是一种授权协议，用于在客户端和服务器之间安全地共享资源。

OAuth2.0 解决了第三方应用程序获取用户数据的问题，它允许用户将自己的数据与第三方应用程序进行共享，同时保护用户的隐私和安全。

在 OAuth2.0 中，用户通过授权向第三方应用程序提供访问权限，而不是直接共享其凭据（如用户名和密码）。第三方应用程序通过向授权服务器请求访问令牌（access token），然后使用该令牌来访问受保护的资源服务器（如用户数据服务器）。

互联网社区的工程师们于2012年制定了OAuth 2.0，并将其发布为（[RFC6749](https://datatracker.ietf.org/doc/html/rfc6749)和[RFC6750](https://datatracker.ietf.org/doc/html/rfc6750)）。

OAuth 2.0框架中的关键方面被有意保持开放和可扩展性。基本的RFC 6749规范了四种安全角色，并引入了四种被称为授权授予的方式，供客户端获取访问令牌。其余部分，比如令牌中包含了什么，留给实施者或未来的扩展来填补。

### OAuth 术语表

- 资源所有者（Resource Owner）：拥有客户端应用程序想要访问的数据的用户。
- 客户端（Client）：想要访问用户数据的的应用程序
- 授权服务端（Authorization Server）：通过用户许可，授权客户端访问用户数据的授权服务端。
- 资源服务端（Resource Server）：存储客户端要访问的数据的系统。在某些情况下，资源服务端和授权服务端是同一个服务端。

授权服务器通常和资源服务器可以是不同的实体，也可以是同一个实体。在一些情况下，授权服务器和资源服务器可能由同一提供商或服务提供商管理，以简化集成和提供一致的身份验证和资源访问体验。然而，它们可以独立地进行部署和管理，只要在OAuth2.0协议框架下能够正确地进行身份验证和授权即可。

### 流程图

图例地址：[RFC 6749 - The OAuth 2.0 Authorization Framework (ietf.org)](https://datatracker.ietf.org/doc/html/rfc6749#section-1.2)

```
 +--------+                               +---------------+
     |        |--(A)- Authorization Request ->|   Resource    |
     |        |                               |     Owner     |
     |        |<-(B)-- Authorization Grant ---|               |
     |        |                               +---------------+
     |        |
     |        |                               +---------------+
     |        |--(C)-- Authorization Grant -->| Authorization |
     | Client |                               |     Server    |
     |        |<-(D)----- Access Token -------|               |
     |        |                               +---------------+
     |        |
     |        |                               +---------------+
     |        |--(E)----- Access Token ------>|    Resource   |
     |        |                               |     Server    |
     |        |<-(F)--- Protected Resource ---|               |
     +--------+                               +---------------+
```

以下是 OAuth 2.0 抽象流程图，让我们一起看看上述术语在图中的应用

![image.png](images/202307101527855.png)
授权密钥（Authorization Key）或者权限（Grant）可以是授权码或者令牌的类型。下文我们将会提到不同的权限和授权密钥。现在，让我们先详细解释授权的流程。

  
1. 用户通过点击按钮启动整个授权流程。这个按钮通常类似于“谷歌登陆“、”Facebook 登陆“或者通过其他的应用登陆。
2. 然后客户端将用户重定向到授权服务端。在重定向的过程中，客户端将类似客户 ID、重定向 URI 的信息发送给授权服务端。
3. 授权服务端处理用户认证，并显示授权许可窗口，然后从用户方获得授权许可。如果你通过谷歌登陆，你必须向谷歌，而不是客户端，提供登陆证书——例如向 accounts.google.com 提供登陆证书。
4. 如果用户授权许可，则授权服务端将用户重定向到客户端，同时发送授权密钥（授权码或令牌）。
5. 客户端向资源服务端发送包含授权密钥的请求，要求资源服务端返回用户数据。
6. 资源服务端验证授权密钥，并向客户端返回它所请求的数据。

这就是用户在不提供密码的情况下，允许第三方应用访问用户数据的过程。但与此同时，有一些问题出现了：
- 我们如何限制客户端只访问资源服务端上的部分数据？
- 如果我们只希望客户端读取数据，而没有权限写入数据呢？
这些问题将我们引导至 OAuth 技术术语中另一部分很重要的概念：授权范围（Scope）。

### OAuth 中的授权范围（Scope）

在 OAuth 2.0 中，授权范围用于限制应用程序访问某用户的数据。这是通过发布仅限于用户授权范围的权限来实现的。

当客户端向授权服务端发起权限请求时，它同时随之发送一个授权范围列表。授权客户端根据这个列表生成一个授权许可窗口，并通过用户授权许可。如果用户同意了其授权告知，授权客户端将发布一个令牌或者授权码，该令牌或授权码仅限于用户授权的范围。

举个例子，如果我授权了某客户端应用访问我的谷歌通讯录，则授权服务端向该客户端发布的令牌不能用于删除我的联系人，或者查看我的谷歌日历事件——因为它仅限于读取谷歌通讯录的范围。

### Client 类型

OAuth定义了两种类型的客户端，根据它们与授权服务器进行安全认证的能力而定：

保密 - 具有凭据的客户端与服务器之间唯一标识，并且与用户和其他实体保持机密。在web服务器上执行的Web应用程序可以是此类客户端。

公开 - 没有凭据的客户端。在移动设备上执行的应用程序或在浏览器中执行（SPA）可以是此类客户端。

### 端点

Authorization Endpoint

这是服务器的端点，用于对最终用户进行身份验证，并在授权码和隐式流程（授权）中向请求的客户端授予权限。

Token Endpoint

令牌终端点允许客户端交换有效的授权凭证，例如从授权终端点获取的代码，以获取访问令牌。

也可以发放刷新令牌，以允许客户端在访问令牌过期时获取新的访问令牌，而无需重新提交原始授权凭证的新实例，如代码或资源所有者密码凭证。这样做的目的是最大限度地减少用户与授权服务器的交互，并提升整体体验。

如果客户端是保密的，将需要在令牌终端点进行身份验证。



### OAuth2.0 几种流程

OAuth 2.0 规范中定义了以下几种授权流程（flows）：

1. 授权码授权流程 (Authorization Code Flow)：客户端通过重定向用户的浏览器到授权服务器，用户登录并授权，授权服务器返回一个授权码给客户端，然后客户端使用授权码向授权服务器请求令牌 (access token)。
2. 隐式授权流程 (Implicit Flow)：与授权码授权流程类似，但是省略了获取授权码的步骤，直接将令牌返回给客户端，并通过前端重定向方式传递给用户。
3. 密码凭证授权流程 (Resource Owner Password Credentials Flow)：客户端通过用户提供的用户名和密码直接向授权服务器请求令牌，适用于高度受信任的客户端应用程序之间的授权交互。
4. 客户端凭证授权流程 (Client Credentials Flow)：客户端使用自己的凭证（如客户端 ID 和密钥）向授权服务器请求令牌，该流程通常用于客户端代表自己访问资源的情况，而不是为用户访问资源。

##  授权码流程

图例地址：[RFC 6749 - The OAuth 2.0 Authorization Framework (ietf.org)](https://datatracker.ietf.org/doc/html/rfc6749#section-4.1)

```
 +----------+
 | Resource |
 |   Owner  |
 |          |
 +----------+
	  ^
	  |
	 (B)
 +----|-----+          Client Identifier      +---------------+
 |         -+----(A)-- & Redirection URI ---->|               |
 |  User-   |                                 | Authorization |
 |  Agent  -+----(B)-- User authenticates --->|     Server    |
 |          |                                 |               |
 |         -+----(C)-- Authorization Code ---<|               |
 +-|----|---+                                 +---------------+
   |    |                                         ^      v
  (A)  (C)                                        |      |
   |    |                                         |      |
   ^    v                                         |      |
 +---------+                                      |      |
 |         |>---(D)-- Authorization Code ---------'      |
 |  Client |          & Redirection URI                  |
 |         |                                             |
 |         |<---(E)----- Access Token -------------------'
 +---------+       (w/ Optional Refresh Token)
```

授权码流程，或者说授权码权限，是理想的 OAuth 流程。它被认为是非常安全的，因为它同时使用前端途径（浏览器）和后端途径（服务器）来实现 OAuth2.0 机制。

![image.png](images/202307101535667.png)

1. 客户端重定向到授权服务器：客户端向授权服务器发送请求，包括客户端标识（Client ID）、请求的范围（Scope）、响应类型设置为 "code" 并提供回调 URL（Redirect URI）。
2. 用户登录并授权访问：用户通过授权服务器提供的登录界面进行身份验证，如果验证成功，将看到一个授权页面。用户选择是否授予客户端访问其受保护资源的权限。
3. 授权服务器颁发授权码：如果用户授权客户端访问其受保护资源，授权服务器将生成一个授权码，并将其发送回客户端的回调 URL。
4. 客户端通过授权码请求访问令牌：客户端在后端服务器使用授权码向授权服务器请求访问令牌。请求中包括客户端标识、客户端的机密信息（如密钥）、授权码、回调 URL。
5. 授权服务器验证授权码并颁发访问令牌：授权服务器接收到客户端发送的请求后，验证授权码的有效性和合法性。如果验证通过，授权服务器将颁发一个访问令牌及可选的刷新令牌给客户端。
6. 客户端使用访问令牌访问资源服务器：客户端获得访问令牌后，可以使用该令牌向资源服务器发送请求来访问受保护资源。请求中需要包含访问令牌作为身份凭证。
7. 资源服务器验证访问令牌并返回受保护资源：资源服务器接收到客户端发送的请求后，会验证访问令牌的有效性和权限。如果访问令牌有效且具有足够的权限，资源服务器将返回请求的受保护资源给客户端。

### 授权请求

客户端通过将用户重定向到授权服务端来发起一个授权流程，其中，`response_type`需被设置成`code`。这告知了授权服务端用授权码来响应。

对应步骤（A），客户端提供以下参数请求Authorization Server：

1. `response_type`：必需。指定授权服务器返回的授权类型，对于授权码模式，该值应为`code`。
2. `client_id`：必需。在授权服务器注册的客户端应用程序的唯一标识符。
3. `redirect_uri`：可选。授权服务器授权后重定向回客户端应用程序的URL。
4. `scope`：可选。请求的访问权限范围，用空格分隔的字符串。
5. `state`：推荐。客户端生成的随机字符串，用于防止跨站请求伪造（CSRF）攻击。

示例如下：

```
GET /authorize?
	response_type=code&
	client_id=s6BhdRkqt3&
	state=xyz&
	redirect_uri=https%3A%2F%2Fclient%2Eexample%2Ecom%2Fcb HTTP/1.1
Host: server.example.com
```


### 授权响应

资源拥有者授权了请求之后，授权服务器通过重定向回调URL返回响应参数。客户端应用程序可以通过解析URL来获取授权码和状态值。

对应步骤（C），Authorization Server会返回如下信息：

1. `code`：授权码，用于交换访问令牌的临时凭证。
2. `state`：如果在请求中提供了`state`参数，授权服务器会在重定向回客户端应用程序时包含该参数的原始值。用于客户端验证请求和响应之间的一致性。

示例如下：

```
HTTP/1.1 302 Found
     Location: https://client.example.com/cb?
     code=SplxlOBeZQQYbYS6WxSbIA&
     state=xyz
```

Location头部信息指向步骤（A）提供的redirect_uri地址，同时携带code信息和state信息给client，这样浏览器在重定向的时候就会已GET的方式访问Client提供的redirect_uri，同时Client接收到code信息和state信息。下一步就可以请求access_token了。


### 访问令牌请求

协议地址：[RFC 6749 - The OAuth 2.0 Authorization Framework (ietf.org)](https://datatracker.ietf.org/doc/html/rfc6749#section-4.1.3)

对应步骤（D），客户端提供以下参数请求Authorization Server：

1. `grant_type`：必需。授权类型，对于授权码模式，该值应为`authorization_code`。
2. `code`：必需。授权码（即先前获得的授权码）。
3. `redirect_uri`：必需。在授权过程中指定的回调URL。
4. `client_id`：必需。在授权服务器注册的客户端应用程序的唯一标识符。

示例如下：

示例中没有给出`client_id`参数，如果客户端类型是机密的，或者客户端被分配了客户端凭证（或被指定了其他认证要求），那么必须在获取Token时候对客户端进行认证。

```
POST /token HTTP/1.1
     Host: server.example.com
     Authorization: Basic czZCaGRSa3F0MzpnWDFmQmF0M2JW
     Content-Type: application/x-www-form-urlencoded

     grant_type=authorization_code&code=SplxlOBeZQQYbYS6WxSbIA
     &redirect_uri=https%3A%2F%2Fclient%2Eexample%2Ecom%2Fcb
```

### 访问令牌响应 

如果access token验证通过，授权服务颁发access token和 可选的刷新token给客户端。对应步骤（E），Authorization Server会返回如下典型的信息：

1. `access_token`：访问令牌，用于访问受保护的资源。
2. `token_type`：令牌类型，通常是Bearer。
3. `expires_in`：访问令牌的有效时间（以秒为单位）。
4. `refresh_token`：刷新令牌，用于获取新的访问令牌。
5. `scope`：访问权限范围，如果授权请求中包含该参数。

```
 HTTP/1.1 200 OK
     Content-Type: application/json;charset=UTF-8
     Cache-Control: no-store
     Pragma: no-cache

     {
       "access_token":"2YotnFZFEjr1zCsicMWpAA",
       "token_type":"example",
       "expires_in":3600,
       "refresh_token":"tGzv3JOkF0XG5Qx2TlKWIA",
       "example_parameter":"example_value"
     }
```

### 为什么用授权码来交换令牌？

访问令牌是唯一能用于访问资源服务端上的数据的东西，而不是授权码。所以为什么在客户端实际需要访问令牌的情况下，将`response_type`设置成授权码呢？这是因为这样做能使 OAuth 流程非常安全。

![image.png](images/202307101539997.png)
_**问题**_：访问令牌是我们不希望任何人能访问的秘密信息。如果客户端直接请求访问令牌，并将其存储在浏览器里，它可能会被盗，因为浏览器并不是完全安全的。任何人都能看见网页的代码，或者使用开发工具来获取访问令牌。

_**解决方案**_：未了避免将访问令牌暴露在浏览器中，客户端的前端从授权服务端获得授权码，然后发送这个授权码到客户端的后端。现在，为了用授权码交换访问令牌，我们需要一个叫做客户密码（client_secret）的东西。这个客户密码只有客户端的后端知道，然后后端向授权服务端发送一个 POST 请求，其中包含了授权码和客户密码。这个请求可能如下所示：

授权服务端会验证客户密码和授权码，然后返回一个访问令牌。后端程序存储了这个访问令牌并且可能使用此令牌来访问资源服务端。这样一来，浏览器就无法读取访问令牌了。

## 客户端凭据授权

图例地址：[RFC 6749 - The OAuth 2.0 Authorization Framework (ietf.org)](https://datatracker.ietf.org/doc/html/rfc6749#section-4.4)

```
     +---------+                                  +---------------+
     |         |                                  |               |
     |         |>--(A)- Client Authentication --->| Authorization |
     | Client  |                                  |     Server    |
     |         |<--(B)---- Access Token ---------<|               |
     |         |                                  |               |
     +---------+                                  +---------------+
```

(A) 客户端向授权服务器进行身份验证，并从令牌端点请求访问令牌。

(B) 授权服务器对客户端进行身份验证，如果验证通过，则发放访问令牌。

### 访问令牌请求

请求示例如下：

1. `grant_type`: 参数一定为 client_credentials
2. `scope`: 可选参数。

示例如下：

```
 POST /token HTTP/1.1
 Host: server.example.com
 Authorization: Basic czZCaGRSa3F0MzpnWDFmQmF0M2JW
 Content-Type: application/x-www-form-urlencoded

 grant_type=client_credentials
```

### 访问令牌响应

Access Token Response和Authorization Code一致，就不列出来了。

## 隐式流程

当你没有后端程序，并且你的网站是一个仅使用浏览器的静态网站时，应该使用 OAuth2.0 隐式流程。在这种情况下，当你用授权码交换访问令牌时，你跳过发生在后端程序的最后一步。在隐式流程中，授权服务端直接返回访问令牌。

![image.png](images/202307101541268.png)

客户端将浏览器重定向到授权服务端 URI，并将`response_type`设置成`token`，以启动授权流程。授权服务端处理用户的登录和授权许可。请求的返回结果是访问令牌，客户端可以通过这个令牌访问资源服务端。

隐式流程被认为不那么安全，因为浏览器负责管理访问令牌，因此令牌有可能被盗。尽管如此，它仍然被单页应用广泛使用。

### Authorization Request

客户端提供以下参数请求Authorization Server：

1. response_type：必选。值固定为“token”。
2. client_id：必选。第三方应用的标识ID。
3. state：**推荐**。Client提供的一个字符串，服务器会原样返回给Client。
4. redirect_uri：可选。授权成功后的重定向地址。
5. scope：可选。表示授权范围。

重点区别在于**response_type为“token”**，而不再是“code”，redirect_uri也变为了可选参数。

比如以下示例：

```
GET /authorize?response_type=token&client_id=1&state=xyz&redirect_uri=https%3A%2F%2Fclient%2Eexample%2Ecom%2Foauth2&scope=user,photo HTTP/1.1
Host: server.example.com
```

### Access Token Response

Authorization Server会返回如下典型的信息：

1. access_token：访问令牌。
2. refresh_token：刷新令牌。
3. expires_in：过期时间。

示例如下：

```
HTTP/1.1 302 Found
Location: http://client.example.com/oauth2#access_token=2YotnFZFEjr1zCsicMWpAA&state=xyz&expires_in=3600
```

注意其和Authorization Code的最大区别在于它是把token信息放在了url的hash部分（#后面），而不是作为参数(?后面)。这样浏览器在访问重定向的Location指定的url时，就不会把这些数据发送到服务器。而Client可以通过读取Location头信息中获取到access_token信息。


## 资源拥有者密码凭证授权

```
 +----------+
 | Resource |
 |  Owner   |
 |          |
 +----------+
	  v
	  |    Resource Owner
	 (A) Password Credentials
	  |
	  v
 +---------+                                  +---------------+
 |         |>--(B)---- Resource Owner ------->|               |
 |         |         Password Credentials     | Authorization |
 | Client  |                                  |     Server    |
 |         |<--(C)---- Access Token ---------<|               |
 |         |    (w/ Optional Refresh Token)   |               |
 +---------+                                  +---------------+
```

这种模式再一步简化，和Authorzation Code类型下重要的区分就是省略了Authorization Request和Authorization Response。而是Client直接使用Resource owner提供的username和password来直接请求access_token（直接发起Access Token Request然后返回Access Token Response信息）。这种模式一般适用于Resource server高度信任第三方Client的情况下。

### 访问令牌请求

客户端提供以下参数请求Authorization Server：

1. grant_type：必选。值固定为“password”。
2. username：必选。用户登陆名。
3. passward：必选**。**用户登陆密码。
4. scope：可选。表示授权范围。

比如以下示例：

```
POST /token HTTP/1.1
Host: server.example.com
Content-Type: application/x-www-form-urlencoded

grant_type=password&username=blackheart&password=1234
```

### 访问令牌响应

Access Token Response和Authorization Code一致，就不列出来了。

## 刷新令牌

在上述得到访问令牌（access_token）时，一般会提供一个过期时间和刷新令牌。以便在访问令牌过期失效的时候可以由客户端自动获取新的访问令牌，而不是让用户再次登陆授权。那么问题来了，是否可以把过期时间设置的无限大呢，答案是可以的，但是这样非常不安全。

如下是刷新令牌的收客户端需要提供给Authorization Server的参数：

1. grant_type：必选。固定值“refresh_token”。
2. refresh_token：必选。客户端得到access_token的同时拿到的刷新令牌。

比如以下示例：

```
POST /token HTTP/1.1
Host: server.example.com

grant_type=refresh_token&refresh_token=tGzv3JOkF0XG5Qx2TlKWIA
```


## Token传递方式

在第三方Client拿到access_token后，如何发送给Resouce Server这个问题并没有在RFC6729种定义，而是作为一个单独的RFC6750来独立定义了。这里做以下简单的介绍，主要有三种方式如下：

1. URI Query Parameter.
2. Authorization Request Header Field.
3. Form-Encoded Body Parameter.

### URI Query Parameter

这种使用途径应该是最常见的一种方式，非常简单，比如：

```
GET /resource?access_token=mF_9.B5f-4.1JqM HTTP/1.1

Host: server.example.com
```

在我们请求受保护的资源的Url后面追加一个access_token的参数即可。另外还有一点要求，就是Client需要设置以下Request Header的**Cache-Control:no-store**，用来阻止access_token不会被Web中间件给log下来，属于安全防护方面的一个考虑。

### Authorization Request Header Field

因为在HTTP应用层协议中，专门有定义一个授权使用的Request Header，所以也可以使用这种方式：

```
GET /resource HTTP/1.1
Host: server.example.com
Authorization: Bearer mF_9.B5f-4.1JqM
```

### Form-Encoded Body Parameter

使用Request Body这种方式，有一个额外的要求，就是Request Header的"Content-Type"必须是固定的“application/x-www-form-urlencoded”，此外还有一个限制就是不可以使用GET访问，这个好理解，毕竟GET请求是不能携带Request Body的。

```
POST /resource HTTP/1.1
Host: server.example.com
Content-Type: application/x-www-form-urlencoded
  
access_token=mF_9.B5f-4.1JqM
```