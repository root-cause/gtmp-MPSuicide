var pistolGroupHash = API.getHashKey("GROUP_PISTOL");

var animInProgress = false;
var animName = "";
var animTime = 1.0;
var shotFired = false;
var startTime = 0;

API.onServerEventTrigger.connect(function(eventName, args) {
    if (eventName == "SuicideAnimReporter")
    {
        animInProgress = args[0];
        animName = args[1];
        animTime = args[2];
        shotFired = false;
        startTime = API.getGlobalTime();
    }
});

API.onKeyDown.connect(function(e, key) {
    if (key.KeyCode == Keys.K)
    {
        if (API.isChatOpen() || API.isPlayerInAnyVehicle(API.getLocalPlayer()) || animInProgress) return;

        var currentWeapon = API.getPlayerCurrentWeapon();
        var usePistol = (API.returnNative("GET_WEAPONTYPE_GROUP", 0, currentWeapon) == pistolGroupHash && API.returnNative("GET_AMMO_IN_PED_WEAPON", 0, API.getLocalPlayer(), currentWeapon) > 0);
        API.triggerServerEvent("Suicide_Begin", usePistol);
    }
});

API.onUpdate.connect(function() {
    if (!animInProgress) return;

    if (!shotFired && animName == "PISTOL")
    {
        if (API.returnNative("HAS_ANIM_EVENT_FIRED", 8, API.getLocalPlayer(), API.getHashKey("Fire")))
        {
            API.triggerServerEvent("Suicide_Shoot");
            shotFired = true;
        }
    }

    if (API.getAnimCurrentTime(API.getLocalPlayer(), "MP_SUICIDE", animName) >= animTime)
    {
        animInProgress = false;
        API.setPlayerHealth(-1);
    }

    if (API.getGlobalTime() - startTime >= 6500) // reset stuff because player somehow failed the suicide
    {
        animInProgress = false;
        API.triggerServerEvent("Suicide_Cancel");
    }
});