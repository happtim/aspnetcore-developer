<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
</Query>


//Deserializing Partial JSON Fragments
//通常，在处理大型 JSON 文档时，您只对小型文档感兴趣 信息片段。
//当您想要反序列化时，这种情况可能会很烦人 该 JSON 片段为 .NET 对象，因为您必须为 整个 JSON 结果。


void Main()
{
	string googleSearchText = @"{
	  'responseData': {
	    'results': [
	      {
	        'GsearchResultClass': 'GwebSearch',
	        'unescapedUrl': 'http://en.wikipedia.org/wiki/Paris_Hilton',
	        'url': 'http://en.wikipedia.org/wiki/Paris_Hilton',
	        'visibleUrl': 'en.wikipedia.org',
	        'cacheUrl': 'http://www.google.com/search?q=cache:TwrPfhd22hYJ:en.wikipedia.org',
	        'title': '<b>Paris Hilton</b> - Wikipedia, the free encyclopedia',
	        'titleNoFormatting': 'Paris Hilton - Wikipedia, the free encyclopedia',
	        'content': '[1] In 2006, she released her debut album...'
	      },
	      {
	        'GsearchResultClass': 'GwebSearch',
	        'unescapedUrl': 'http://www.imdb.com/name/nm0385296/',
	        'url': 'http://www.imdb.com/name/nm0385296/',
	        'visibleUrl': 'www.imdb.com',
	        'cacheUrl': 'http://www.google.com/search?q=cache:1i34KkqnsooJ:www.imdb.com',
	        'title': '<b>Paris Hilton</b>',
	        'titleNoFormatting': 'Paris Hilton',
	        'content': 'Self: Zoolander. Socialite <b>Paris Hilton</b>...'
	      }
	    ],
	    'cursor': {
	      'pages': [
	        {
	          'start': '0',
	          'label': 1
	        },
	        {
	          'start': '4',
	          'label': 2
	        },
	        {
	          'start': '8',
	          'label': 3
	        },
	        {
	          'start': '12',
	          'label': 4
	        }
	      ],
	      'estimatedResultCount': '59600000',
	      'currentPageIndex': 0,
	      'moreResultsUrl': 'http://www.google.com/search?oe=utf8&ie=utf8...'
	    }
	  },
	  'responseDetails': null,
	  'responseStatus': 200
	}";

	JObject googleSearch = JObject.Parse(googleSearchText);

	// get JSON result objects into a list
	IList<JToken> results = googleSearch["responseData"]["results"].Children().ToList();

	// serialize JSON results into .NET objects
	IList<SearchResult> searchResults = new List<SearchResult>();
	foreach (JToken result in results)
	{
		// JToken.ToObject is a helper method that uses JsonSerializer internally
		SearchResult searchResult = result.ToObject<SearchResult>();
		searchResults.Add(searchResult);
	}
	
	searchResults.Dump();
}


public class SearchResult
{
	public string Title { get; set; }
	public string Content { get; set; }
	public string Url { get; set; }
}
