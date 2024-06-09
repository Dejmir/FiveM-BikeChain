RegisterNetEvent("BikeChain:client:AddQtargetChain")
AddEventHandler("BikeChain:client:AddQtargetChain", function(entity)
    exports['qb-target']:AddTargetEntity(entity, { -- The specified entity number
            options = { -- This is your options table, in this table all the options will be specified for the target to accept
                { -- This is the first table with options, you can make as many options inside the options table as you want
                    num = 1,
                    label = 'Put chain back', -- This is the label of this option which you would be able to click on to trigger everything, this has to be a string
                    icon = 'fa fa-chain-broken',
                    action = function(entity) -- This is the action it has to perform, this REPLACES the event and this is OPTIONAL  
                        TriggerEvent("BikeChain:client:PutChainBackFromQtarget", entity)
                    end,
                }
            },
            distance = 1.3, -- This is the distance for you to be at for the target to turn blue, this is in GTA units and has to be a float value
        })
end)
RegisterNetEvent("BikeChain:client:AddQtargetSpray")
AddEventHandler("BikeChain:client:AddQtargetSpray", function(entity)
    exports['qb-target']:AddTargetEntity(entity, { -- The specified entity number
            options = { -- This is your options table, in this table all the options will be specified for the target to accept
                { -- This is the first table with options, you can make as many options inside the options table as you want
                    num = 2,
                    label = 'Spray with WD40', -- This is the label of this option which you would be able to click on to trigger everything, this has to be a string
                    icon = 'fa fa-tint',
                    item = 'wd40',
                    action = function(entity) -- This is the action it has to perform, this REPLACES the event and this is OPTIONAL  
                        TriggerEvent("BikeChain:client:SprayFromQtarget", entity)
                    end,
                }
            },
            distance = 1.3, -- This is the distance for you to be at for the target to turn blue, this is in GTA units and has to be a float value
        })
end)

RegisterNetEvent("BikeChain:client:RemoveQtargetChain")
AddEventHandler("BikeChain:client:RemoveQtargetChain", function(entity)
    exports['qb-target']:RemoveTargetEntity(entity, 'Put chain back')
end)
RegisterNetEvent("BikeChain:client:RemoveQtargetSpray")
AddEventHandler("BikeChain:client:RemoveQtargetSpray", function(entity)
    exports['qb-target']:RemoveTargetEntity(entity, 'Spray with WD40')
end)