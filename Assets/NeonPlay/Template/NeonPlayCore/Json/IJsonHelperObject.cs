namespace NeonPlay.Json {
	
	public interface IJsonHelperObject {

		string SerialiseObject(object jsonObject, bool includePrivateVariables = false, JsonTypeHandling typeHandling = JsonTypeHandling.Auto, bool indented = false);

		TObject DeserialiseObject<TObject>(string json, bool includePrivateVariables = false, JsonTypeHandling typeHandling = JsonTypeHandling.Auto);
	}
}
