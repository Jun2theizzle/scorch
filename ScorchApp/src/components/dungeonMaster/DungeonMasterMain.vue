<template>
    <div class="dm-main-component">
        <div class="d-flex dm-character-view">
                <div class="item-searcher border border-dark flex-column">
                    <searcher @search-row-selected="searchItem" :search-data="searchItems" :limit-per-page="10" :column-keys="columnKeys"/>
                </div>
                <div class="middle-ops d-flex flex-column">
                    <div class="character-operations">

                      <h3>Shop Status</h3>
                      <toggle-button @change="toggleShopStatus" v-model="storeStatus" :value="false" :sync="true" :width="65" :labels="{checked: 'Open', unchecked: 'Closed'}" />
                    <p>
                        <button class="btn btn-warning" @click="rest">Rest</button>
                    </p>

                        <character-operator v-on:reset="selectedChars = []" :character-list="selectedChars" :item="selectedItem"></character-operator>
                    </div>

                    <div v-if="selectedItem.ItemId" class="item-card">
                        <div  class ="border border-dark item-card-inner">
                            <div class="item-card-detail">
                                <item-card :item="selectedItem"></item-card>
                            </div>
                            <div class="item-qty-entry d-flex">
                                <span class="item-qty-label">Quantity </span>
                                <div class="input-group">
                                    <input type="number" class="form-control" id="item-quantity" v-model="itemQty" placeholder="# Items" autocomplete="off"/>
                                    <button class="btn btn-warning" @click="giveItem()">Give Item</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            <div class="flex-column dm-party-view">
                <div class="dm-character-cards-container" v-for="(char, index) in party" @click="toggleCharacter(char)" :key="index">
                    <character-tile :character="char" v-bind:class="{ selected: isCharacterSelected(char) }"></character-tile>
                </div>
            </div>

        </div>
    </div>

</template>

<script>
    import { CharacterTile } from 'components/character'
    import CharacterOperator from './operators/CharacterOperator.vue'
    import { Searcher } from 'components/util'
    import { ItemCard } from 'components/items'
    import { ItemService, CharacterService } from 'services'

export default {
    name : 'dm-main-component',

    data() {
        return {
            selectedChars: [],
            selectedItem: {},
            columnKeys: ['Name', 'ItemClass', 'Cost', 'AC', 'Damage', 'Slot'],
            itemQty: 1,
            storeStatus: false
        }
    },
    async created() {
      await this.$store.dispatch('getParty');
      await this.$store.dispatch('getDisplayItems');
    },
    methods: {
        async rest() {
            for(let char of this.party)
            {
                let payload = {};
                payload.body = {};
                payload.characterId = char.CharacterId;
                // Set their hp to the max hp
                payload.body.Hp = char.Hp > char.MaxHp ? char.Hp : char.MaxHp;
                if(char.SpellSlots){
                    // Restore their spell slots
                    payload.body.SpellSlots = [];
                    for(let spellSlot of char.SpellSlots) {
                        let spellSlotPayload = {
                            Count: spellSlot.Count,
                            SpellLevel: spellSlot.SpellLevel,
                            Available: spellSlot.Count
                        };
                        payload.body.SpellSlots.push(spellSlotPayload);
                    }
                }
                await this.$store.dispatch('updateCharacter', payload);
            };
            this.$logging.update();
        },
        async openShop() {
            this.$logging.action({action: 'showShop'});
        },
        async closeShop() {
          this.$logging.action({action: 'closeShop'});
        },
        toggleShopStatus() {
          if (this.storeStatus) {
            this.openShop();
          } else {
            this.closeShop();
          }
        },
        toggleCharacter(character) {
            var foundChar = this.selectedChars.find(char => char.CharacterId == character.CharacterId);

            if(foundChar){
                var index = this.selectedChars.findIndex(char => char.CharacterId == character.CharacterId);
                this.selectedChars.splice(index, 1);
            }
            else{
                this.selectedChars.push(character);
            }
        },
        isCharacterSelected(character) {
            return !(this.selectedChars.find(char => char.CharacterId == character.CharacterId) == undefined);
        },
        async searchItem(item){
            if(item){
                let response = {};
                response = await ItemService.getItemById(item.ItemId);
                this.selectedItem = response.body;
            }
        },
        async giveItem(){
            if(this.selectedChars.length === 0){
                this.$notify.warning('Please select at least one character.');
            }
            let nameList = this.selectedChars.map(char => {return char.Firstname + ' ' + char.Lastname}).join(', ');
            let successMsg = `Successfully gave ${this.itemQty} ${this.selectedItem.Name} to ${nameList}`;
            for(let char of this.selectedChars){
                if(this.selectedItem.ItemId && this.itemQty > 0){
                    let itemAdded = {};
                    itemAdded.ItemId = this.selectedItem.ItemId;
                    itemAdded.Count = parseInt(this.itemQty);
                    try {
                        await CharacterService.postCharacterItem(char.CharacterId, itemAdded);
                        let successPayload = {
                            user: char.CharacterId,
                            type: 'success',
                            message: `You received ${this.itemQty} ${this.selectedItem.Name}.`
                        };
                        this.$logging.notify(successPayload);
                        this.$notify.success(successMsg);
                        successPayload = {
                            characterId: char.CharacterId,
                            message: `You received ${this.itemQty} ${this.selectedItem.Name}.`
                        }
                        this.$notify.success(successMsg);
                        this.$socket.emit('updateParty');
                    }
                    catch(errorResponse){
                        console.log(`Failed to add item to char : ${char.CharacterId} error : ${errorResponse.bodyText}`);
                        this.$notify.failure('Failed to give item to characters');
                    }
                }
            }
        }
    },
    computed: {
        party() {
            return this.$store.getters.myParty;
        },
        searchItems() {
            return this.$store.getters.items;
        }
    },
    components: {
        CharacterTile,
        CharacterOperator,
        Searcher,
        ItemCard
    }
}
</script>

<style lang="scss" scoped>
    div {
        box-sizing: border-box;
    }

    .dm-main-component {
        height: 600px;
    }
    .dm-character-view {
        flex-wrap: wrap;
    }
    .dm-party-view {
        margin-top: 1%;
        flex: 1;
        flex-grow: 2;
        .dm-character-cards-container {
     box-sizing:border-box;
           margin-bottom: 1%;
            border-radius: 10px;
        }
    }
    .character-operations {
        margin-top: 3%;
        flex: 1;
        border-radius: 10px;
    }
    .selected {
        box-shadow: 0 0 0 3px black;
    }
    .item-searcher{
        flex: 1;
        flex-grow: 4;
        border-radius: 9px;
        margin-top: 1%;
        input{
            background-color: #E8E8E8;
        }
    }
    .rhs-character-ops{
        flex: 2;
        flex-grow: 5;
        flex-direction:column;
    }
    .middle-ops{
        flex-direction:row;
        flex: 1;
        flex-grow: 2;
        margin-left: 1%;
        margin-right: 1%;
        margin-top: 1%;
    }
    .item-card{
        flex: 3;
    }
    .item-card-inner{
        border-radius: 9px;
        min-height: 60%;
    }
    .item-qty-entry{
        margin: 2%;
        align-items: center;
    }
    .item-qty-label{
        padding-right: 2%;
    }
    .btn-warning{
        border-radius: 0;
    }
</style>
