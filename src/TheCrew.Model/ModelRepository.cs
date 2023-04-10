using System.Text.Json;
using TheCrew.Shared;

namespace TheCrew.Model;

public class ModelRepository
{
   private JsonSerializerOptions _jsonOptions;

   public ModelRepository()
   {
      _jsonOptions = new JsonSerializerOptions();
   }

   public async Task<GameModel> CreateNewAsync()
   {
      var model = new GameModel()
      {
         Id = Guid.NewGuid(),
         Players = new List<PlayerModel>(),
         UnassignedMissionCards = new List<IMissionCardTask>(),
         GenericMissions = new List<IGenericMissionTask>()
      };

      await SaveAsync(model);

      return model;
   }

   public async Task SaveAsync(GameModel model)
   {
        var path = Path.Combine(GetDirectory(), $"{model.Id}.json");

      using Stream writer = new FileStream(path, FileMode.Create);
      await JsonSerializer.SerializeAsync(writer, model, options: _jsonOptions);
   }

   public async Task<GameModel> LoadAsync(Guid id)
   {
        var path = Path.Combine(GetDirectory(), $"{id}.json");

      using Stream reader = new FileStream(path, FileMode.Open);
      return await JsonSerializer.DeserializeAsync<GameModel>(reader, options: _jsonOptions)
         ?? throw new JsonException("Invalid deserialization");
   }

   private static string GetDirectory()
   {
      var dir = new DirectoryInfo(Path.Combine(
         Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
         "SavedGames"));

      if (!dir.Exists)
      {
         dir.Create();
      }

      return dir.FullName;
   }
}
