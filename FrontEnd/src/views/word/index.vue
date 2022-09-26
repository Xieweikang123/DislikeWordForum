<template>
  <el-container>
    <el-aside width="200px" style="    position: absolute;">
      <div class="btnContainer">
        <el-button
          v-for="item in menuBtnList"
          :key="item.type"
          @click="formType = item.type"
          type="primary"
          :plain="formType != item.type"
          >{{ item.txt }}</el-button
        >
        <!-- <el-button
          @click="formType = 'wordList'"
          type="primary"
          :plain="formType != 'wordList'"
          >单词列表</el-button
        >
        <el-button @click="formType = 'MemorizingWord'" type="primary" plain
          >背单词模式</el-button
        >
        <el-button type="warning" plain>数据分析</el-button> -->
      </div>
    </el-aside>
    <el-main>
      <WordForm v-if="formType == 'wordList'"></WordForm>
      <MemorizingWords v-if="formType == 'MemorizingWord'"></MemorizingWords>
    </el-main>
    <!-- 
    <EditForm @RefreshData="GetMyWordList" ref="editForm"></EditForm> -->
  </el-container>
</template>

  <script>
import WordForm from "../word/wordForm.vue";
import MemorizingWords from "../word/MemorizingWords.vue";
export default {
  components: {
    WordForm,
    MemorizingWords,
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