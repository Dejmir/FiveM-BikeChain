# FiveM Bike Chain
### <b>[📜]</b> Description: 
FiveM script improving immersion by simulating bike chains rusting and popping out.<br>

### <b>[📼]</b> Video:
[![Watch the video](https://img.youtube.com/vi/IoIb5PzYM5s/mqdefault.jpg)](https://www.youtube.com/watch?v=IoIb5PzYM5s)

### </b>[✨]</b> Features:
- Detailed JSON config.
- After *defined* amount of seconds, chain of the bike can pop out. If not, chance for that is increased by *defined* percentage amount.
- If chain has popped out, bike is unusable and undrivable. You can only push bike by holding forward key(W) while character are entering it.
- You can put chain back on its place with *defined* amount of time, animation and prop.
- To decrease chain pop chance, you can spray it using WD40 with *defined* amount of time.
- Notifications when: chain is rusty, popped out, back on the place etc.


### <b>[🔗]</b> Other Scripts Dependencies: 
- [ESX framework](https://github.com/esx-framework)
- [qb-target](https://github.com/qbcore-framework/qb-target) (3rd eye)
- [ox_inventory](https://github.com/overextended/ox_inventory) (storing item metadata)

### <b>[📌]</b> Notes:
- **Remember this is a C# resource so even if resource monitor shows high CPU usage in reality its not. Read more [here](https://forum.cfx.re/t/lua-vs-c/644237/5), [here](https://forum.cfx.re/t/c-performance-issue/4839267/3) and [here](https://forum.cfx.re/t/c-question-regarding-resource-usage/5162254/2) If this still does not convince you can consider using [mono v2](https://github.com/thorium-cfx/mono_v2_get_started) by replacing CitizenFX libraries and compiling it by yourself.**
- Used LUA for 3rd eye(qb-target) usage, got not enough time to figure out LUA exports work into C#.

### <b>[⚠️]</b> Bugs:
- Due to fivem re-using network id's, new spawned bike can use already despawned bike network id and its data in result causing chain out of place after spawning.
