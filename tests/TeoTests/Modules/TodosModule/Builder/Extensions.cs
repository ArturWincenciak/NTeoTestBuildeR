using Argon;

namespace TeoTests.Modules.TodosModule.Builder;

internal static class Extensions
{
    public async static Task<object?> Deserialize(this HttpResponseMessage httpResponse)
    {
        if ((await httpResponse.Content.ReadAsStreamAsync()).Length is 0)
            return null;

        var jsonString = await httpResponse.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject(jsonString);
    }
}