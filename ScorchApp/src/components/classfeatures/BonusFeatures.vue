<template>
<div class="character-bonus-features-card">
  <modal v-if="showDetailModal" v-on:close="close">
    <div slot="header">
      <h4>Feature Details</h4>
    </div>
    <div slot="body">
      <div v-for="(details, index) in selectedFeatures" :key="index">
        <h2>{{ details.Name }}</h2>
        <p>{{ details.Description }}</p>
      </div>
    </div>
  </modal>
  <div class="d-flex flex-column">
      <div v-for="(f, key, index) in feature"  
            v-if="levelReq(key, level)"
            @click="showDetails(f)"
          :key="index" 
          class="feature-item border" :style="{ order: getLevel(key) }">
        <small>{{key.replace('_', ' ')}}</small>
        <div v-for="(details, dIndex) in f" :key="dIndex">
          <p>{{ details.Name }}</p>
        </div>
      </div>
    </div>
</div>
</template>

<script>
import { Modal } from 'components/util'
import orderBy from 'lodash/orderBy'
export default {
    name: 'bonus-features',
    props: ['feature', 'level'],
    data() {
      return {
        showDetailModal: false,
        selectedFeatures: {}
      }
    },
    methods: {
      levelReq(featureKey, level) {
        let req = parseInt(featureKey.split('_')[1]);
        return level >= req;
      },
      showDetails(features) {
        this.selectedFeatures = features;
        this.showDetailModal = true;
      },
      getLevel(levelKey) {
        return parseInt(levelKey.split('_')[1]);
      },
      close() {
          $("body").removeClass("modal-open");
          this.showDetailModal = false;
      }
    },
    components: {
      Modal
    }
}
</script>

<style lang="scss" scoped>
@import "~styles/shared.scss";

.feature-item {
    padding: 3% 3% 0 3%;
    border-radius: 10px;
    margin-bottom: 1%;
}
.feature-item:hover {
  cursor: pointer;
}
.character-bonus-features-card {
  margin-bottom: 1%;
}
</style>