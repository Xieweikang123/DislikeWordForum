<template>
  <el-container class="mianContainer">
    <el-aside class="leftside">
      <div class="btnContainer">
        <el-button v-for="item in menuBtnList" :key="item.type" @click="formType = item.type" type="primary"
          :plain="formType != item.type">{{ item.txt }}</el-button>

      </div>
    </el-aside>
    <el-main>
      <WordForm v-if="formType == 'wordList'"></WordForm>
      <MemorizingWords v-if="formType == 'MemorizingWord'"></MemorizingWords>
      <TrendChart v-if="formType == 'TrendChart'"></TrendChart>
    </el-main>
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

  },
  mounted() {
  },
  methods: {},
};
</script>
<style >
@media (max-width: 768px) {
  .paginationStyle {
    margin-bottom: 3%;
  }
  .el-main {
    padding: 0px 10px;
  }
  .mianContainer {
    display: block;
  }
  .leftside {
    position: relative !important;
    border-bottom: 1px dashed #d5d5d5;
    ;
    padding-bottom: 1px;
    margin-bottom: 7px;
    width: inherit !important;
  }

  /* .margin60Auto {
    width: max-content;
  } */

  .el-button {
    padding: 6px 5px;

  }

}

@media (min-width: 768px) {
  .leftside {
    width: min-content !important;
  }
}


.leftside {
  position: absolute;
}


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