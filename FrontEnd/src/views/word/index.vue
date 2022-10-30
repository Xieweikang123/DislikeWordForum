<template>
  <el-container>
    <el-aside width="200px" style="position: absolute">
      <div class="btnContainer">
        <el-button
          v-for="item in menuBtnList"
          :key="item.type"
          @click="formType = item.type"
          type="primary"
          :plain="formType != item.type"
          >{{ item.txt }}</el-button
        >
      </div>
    </el-aside>
    <el-main>
      <WordForm v-if="formType == 'wordList'"></WordForm>
      <MemorizingWords v-if="formType == 'MemorizingWord'"></MemorizingWords>
      <TrendChart v-if="formType == 'TrendChart'"></TrendChart>
    </el-main>
    <!-- 
    <EditForm @RefreshData="GetMyWordList" ref="editForm"></EditForm> -->
  </el-container>
</template>

  <script>
import WordForm from "../word/wordForm.vue";
import MemorizingWords from "../word/MemorizingWords.vue";
import TrendChart from "../word/TrendChart.vue";
export default {
  components: {
    WordForm,
    MemorizingWords,
    TrendChart,
  },
  data() {
    return {
      menuBtnList: [
        {
          txt: "单词列表",
          type: "wordList",
        },
        {
          txt: "背单词模式",
          type: "MemorizingWord",
        },
        {
          txt: "趋势图",
          type: "TrendChart",
        },
      ],
      formType: "wordList",
    };
  },
  watch: {
    // "paging.pageNumber": {
    //   handler(nVal) {
    //     this.GetMyWordList();
    //   },
    // },
    // "paging.searchScop": {
    //   handler(nVal) {
    //     this.GetMyWordList();
    //   },
    // },
  },
  mounted() {
    // this.GetMyWordList();
  },
  methods: {},
};
</script>
<style scoped>
.btnContainer button {
  margin: 7px 9px;
}
/*   
.handPointer {
  cursor: pointer;
}
.btnContainer button {
  margin: 7px 9px;
}
.paginationStyle {
  text-align: right;
  margin-top: 17px;
} */
</style>