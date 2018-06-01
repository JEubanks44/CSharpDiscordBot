using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using System.Collections.Concurrent;
namespace ChaunceyDiscordBot.Services
{
    class AudioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();

        public async Task JoinAudio(IGuild guild, IVoiceChannel voiceChannel)
        {
            
            if (voiceChannel.Guild.Id != guild.Id)
            {
                return; //Fail case
            }

            var audioClient = await voiceChannel.ConnectAsync();

            if (ConnectedChannels.TryAdd(guild.Id, audioClient))
            {
                Console.WriteLine("Voice Channel joined succesfully");
            }
        }

        public async Task LeaveAudio(IGuild guild)
        {
            IAudioClient client;
            if (ConnectedChannels.TryRemove(guild.Id, out client))
            {
                await client.StopAsync();
            }
        }


    }
}
