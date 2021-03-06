﻿using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using DSharpPlus;
using Core_Discord.CoreServices.Interfaces;
using NLog;
using System.IO;
using DSharpPlus.VoiceNext;
using DSharpPlus.EventArgs;
using Core_Discord.CoreServices;
using DSharpPlus.Entities;

namespace Core_Discord.CoreMusic
{
    public class CoreMusicService : IUnloadableService, CoreService
    {
        public const string MusicPath = "data/music";
        private readonly IGoogleApiService apiService;
        private readonly DbService _db;
        private readonly Logger _log;
        private ICoreCredentials _cred;
        private readonly ConcurrentDictionary<long, float> _defaultVolumes;
        private readonly object locker = new object();

        public ConcurrentBag<long> GuildDc;

        private readonly DiscordClient _client;

        public ConcurrentDictionary<long, CoreMusicPlayer> MusicPlayers { get; } = new ConcurrentDictionary<long, CoreMusicPlayer>();



        public CoreMusicService(DiscordClient client,
            DbService db,
            ICoreCredentials cred,
            Core core,
            IGoogleApiService google)
        {
            apiService = google;
            _client = client;
            _db = db;
            _cred = cred;
            _log = LogManager.GetCurrentClassLogger();

            _client.GuildDeleted += Discord_GuildDeleted;
            //create music path
            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), MusicPath)))
            {
                Directory.CreateDirectory(MusicPath);
            }
        }
        public float GetDefaultVolume(long guildId)
        {
            return _defaultVolumes.GetOrAdd(guildId, (id) =>
            {
                using (var uow = _db.UnitOfWork)
                {
                    return uow.GuildConfig.GetOrCreate(guildId, set => set).DefaultMusicVolume;
                }
            });
        }
        //setup based on user 
        //public async Task<CoreMusicPlayer> GetOrCreatePlayer(CommandContext e)
        //{
        //    var gUsr = e.User;
        //    var txtCh = e.Channel;
        //    var vCh = e.Member?.VoiceState?.Channel;
        //    if(vCh == null)
        //    {
        //        _log.Warn($"Voice channel not found or {e.User.Username + e.User.Discriminator} is not connected to one");
        //        await e.RespondAsync($"Voice channel not found or {e.User.Username + e.User.Discriminator} is not connected to one").ConfigureAwait(false);
        //        throw new NotInVOiceChannelException();
        //    }
        //    var vnc = await vCh.ConnectAsync().ConfigureAwait(false);
        //    return (await GetOrCreatePlayer(e.Guild.Id, vnc, txtCh));
        //}
        //public async Task<CoreMusicPlayer> GetOrCreatePlayer(ulong guild, VoiceNextConnection voiceNext, DiscordChannel textChan)
        //{
        //    //return await MusicPlayers.GetOrAdd(guild, _ =>
        //    //{
        //    //    float vol = 1.0f;
        //    //    var avc = voiceNext.Channel;
        //    //    var mp = new CoreMusicPlayer(voiceNext, textChan, apiService, vol, this);

        //    //    DiscordMessage playingMesage = null;
        //    //    DiscordMessage lastFinishedMessage = null;

        //    //    //add implementation for event trigger
        //    //    mp.OnCompleted += (s, song) =>
        //    //    {
        //    //        lastFinishedMessage?.DeleteAsync();
        //    //        try
        //    //        {

        //    //        }
        //    //        catch { }
        //    //    };

        //    //});
        //    return 
        //}
        public CoreMusicPlayer GetPlayerOrDefault(long guildId)
        {
            throw new NotImplementedException();
        }
        public async Task TryQueueRelatedSongAsync(MusicInfo song, DiscordChannel textChan, VoiceNextExtension vch)
        {
            throw new NotImplementedException();
        }
        //event
        private Task Discord_GuildDeleted(GuildDeleteEventArgs e)
        {
            var m = DestroyPlayer((long)e.Guild.Id);
            return Task.CompletedTask;
        }
        /// <summary>
        /// Destory Music Player
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DestroyPlayer(long id)
        {
            _log.Warn("Destorying music player");
            if (MusicPlayers.TryRemove(id, out var mp))
                await mp.Destroy();
        }
        public async Task DestroyAllPlayers()
        {
            foreach (var key in MusicPlayers.Keys)
            {
                await DestroyPlayer(key);
            }
        }
        public Task Unload()
        {
            _client.GuildDeleted -= Discord_GuildDeleted;
            return Task.CompletedTask;
        }
    }
}
