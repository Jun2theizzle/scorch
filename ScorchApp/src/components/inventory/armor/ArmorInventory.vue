<template>
<div class="inventory-card">
  <armor-detail :armor="selectedArmor" :showModal="showDetail" v-on:close="showDetail = false"></armor-detail>
    <div class="item-list scrollbar">
      <div v-for="(armor, index) in armors" :key="index" class="d-flex flex-column list-item border">
        <div @click="armorClick(armor)">
          <item-card :item="armor"></item-card>
        </div>
        <div>
          <button class="btn btn-primary" @click="equipArmor(armor)">
            Equip
          </button>
          <button class="btn btn-warning" @click="sellArmor(armor)">
            Sell
          </button>
          <button class="btn btn-danger" @click="deleteArmor(armor)">
            Delete
          </button>
        </div>
      </div>
    </div>
</div>
</template>

<script>
import ArmorDetail from './ArmorDetail';
import { ItemCard } from 'components/items'
export default {
    name: 'armor-inventory',
    data() {
        return {
            selectedArmor: {},
            showDetail: false
        }
    },
    props: ['armors'],
    methods: {
        armorClick(armor) {
            this.selectedArmor = armor;
            this.showDetail = true;
        },
        equipArmor(armor) {
            this.$emit('equip', armor);
        },
        sellArmor(armor) {
            this.$emit('sell', armor);
        },
        deleteArmor(armor) {
            this.$emit('delete', armor);
        }
    },
    components: {
        ArmorDetail,
        ItemCard
    }
}
</script>

<style lang="scss" scoped>
@import '~styles/shared.scss';

</style>