using System.Text.Json;
using XSDe.Models;
using XSDe.Models.Enums;

namespace XSDe.Tests.TestCases
{
    public class MappingsConfigSerializationTests
    {
        [Fact]
        public void RoundTrip_WithModifierSet_PreservesAllFields()
        {
            var original = new MappingsConfig
            {
                Mappings =
                [
                    new ButtonMapping
                    {
                        Button = XButton.A,
                        Modifier = XButton.LeftShoulder,
                        ActionType = ActionTypes.AppLaunch,
                        Parameter = "spotify.exe",
                        IsLongPress = true,
                        LongPressMilliseconds = 800,
                        DisplayName = "Launch Spotify"
                    }
                ]
            };

            var json = JsonSerializer.Serialize(original);
            var result = JsonSerializer.Deserialize<MappingsConfig>(json);

            Assert.NotNull(result);
            Assert.Single(result.Mappings);

            var mapping = result.Mappings[0];
            var expected = original.Mappings[0];

            Assert.Equal(expected.Id, mapping.Id);
            Assert.Equal(expected.Button, mapping.Button);
            Assert.Equal(expected.Modifier, mapping.Modifier);
            Assert.Equal(expected.ActionType, mapping.ActionType);
            Assert.Equal(expected.Parameter, mapping.Parameter);
            Assert.Equal(expected.IsLongPress, mapping.IsLongPress);
            Assert.Equal(expected.LongPressMilliseconds, mapping.LongPressMilliseconds);
            Assert.Equal(expected.DisplayName, mapping.DisplayName);
        }

        [Fact]
        public void RoundTrip_WithModifierNull_PreservesNull()
        {
            var original = new MappingsConfig
            {
                Mappings =
                [
                    new ButtonMapping
                    {
                        Button = XButton.Start,
                        Modifier = null,
                        ActionType = ActionTypes.SystemCommand,
                        Parameter = "shutdown /s",
                        DisplayName = "Shutdown"
                    }
                ]
            };

            var json = JsonSerializer.Serialize(original);
            var result = JsonSerializer.Deserialize<MappingsConfig>(json);

            Assert.NotNull(result);
            Assert.Null(result.Mappings[0].Modifier);
        }

        [Fact]
        public void RoundTrip_EmptyMappingsList_ProducesEmptyList()
        {
            var original = new MappingsConfig();

            var json = JsonSerializer.Serialize(original);
            var result = JsonSerializer.Deserialize<MappingsConfig>(json);

            Assert.NotNull(result);
            Assert.Empty(result.Mappings);
        }
    }
}