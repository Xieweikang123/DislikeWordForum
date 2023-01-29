<template>
  <div id="noteDrawer">
    <el-drawer
      size="50%"
      ref="noteDrawerRef"
      :title="dynamicTitle"
      custom-class="drawerStyle"
      :visible.sync="isShowDrawer"
      direction="rtl"
      :modal="false"
      :close-on-press-escape="false"
      :before-close="handleClose"
    >
      <div style="margin-bottom: 12px">
        <el-button @click="onConvertUrl" type="primary" size="mini"
          >超链接转换</el-button
        >
        <el-button @click="onGoBackStep" type="primary" size="mini"
          >恢复</el-button
        >
        <!-- <el-button @click="onFormatHTML" type="primary" size="mini"
          >格式化代码</el-button
        > -->
      </div>
      <el-form
        ref="form"
        :model="editRow"
        @submit.native.prevent
        label-width="80px"
      >
        <el-input
          id="myinput"
          ref="contentIpt"
          @paste.native="onContentPaste"
          type="textarea"
          @blur="onContentInputBlur"
          :rows="18"
          v-model="editRow.sayContent"
          @keydown.9.native.prevent="tabFunc"
        ></el-input>
        <el-form-item label="标签:">
          <el-tag
            :key="titem.id"
            v-for="titem in dynamicTags"
            closable
            :disable-transitions="false"
            @close="handleCloseTag(titem)"
          >
            {{ titem.tagName }}
          </el-tag>
          <el-input
            class="input-new-tag"
            v-if="inputVisible"
            v-model="inputValue"
            ref="saveTagInput"
            size="small"
            @keyup.enter.native="handleInputConfirm"
            @blur="handleInputConfirm"
          >
          </el-input>
          <el-button
            v-else
            class="button-new-tag"
            size="small"
            @click="showInput"
            >+ New Tag</el-button
          >
        </el-form-item>
      </el-form>
      <el-row type="flex" class="row-bg" justify="center">
        <el-col :span="6"
          ><el-button type="primary" @click="onSubmit">保存</el-button></el-col
        >
        <el-col :span="2"
          ><div class="grid-content bg-purple-light"></div
        ></el-col>
        <el-col :span="6">
          <el-popconfirm @confirm="confirmDel" title="确定删除吗？">
            <el-button slot="reference" type="danger">删除</el-button>
          </el-popconfirm></el-col
        >
      </el-row>
    </el-drawer>

    <transition name="slide-fade">
      <div v-if="isShowDrawer" class="leftPreview">
        <div style="margin: 30px 14px 13px; display: flex">
          <el-color-picker
            @change="onPickColorChange"
            v-model="pickColor"
            show-alpha
          >
          </el-color-picker>
          <el-button
            @click="onExecCommand('foreColor', pickColor)"
            size="mini"
            plain
            >设置文字颜色</el-button
          >
          <el-button @click="onExecCommand('bold', null)" size="mini" plain
            >加粗</el-button
          >
        </div>
        <div
          id="contentLinePreview"
          class="contentLinePreview"
          @keydown.9.prevent="onExecCommand('indent', null)"
          contenteditable
          v-html="divContent"
        ></div>
      </div>
    </transition>
  </div>
</template>

 <script>
export default {
  data() {
    return {
      isJustNowClose: false,
      pickColor: "#ff4500",
      divContent: "",
      oldContentVal: "",
      iptBlurSelectObj: {},
      dynamicTags: ["标签一", "标签二", "标签三"],
      inputVisible: false,
      inputValue: "",
      editRow: {
        sayContent: "",
      },

      isShowDrawer: false,
    };
  },
  watch: {
    divContent: {
      handler(nVal) {},
    },
    "editRow.sayContent": {
      handler(nVal, oVal) {
        //html 编辑处于焦点时
        if (
          document.activeElement.id == "myinput" ||
          document.activeElement.id == ""
        ) {
          this.divContent = nVal;
        }

        if (oVal && oVal.length > 0) {
          this.oldContentVal = oVal;
        }
      },
    },
    isShowDrawer: {
      handler(nVal) {
        if (!nVal) {
          document.onkeydown = null;
          document.onkeyup = null;
        }
      },
    },
  },
  computed: {
    dynamicTitle() {
      return "内容修改";
    },
  },
  methods: {
    onPickColorChange(nVal) {
      // this.pickColor=nVal
    },
    onExecCommand(commandName, args) {
      document.execCommand(commandName, false, args);
      this.previewHtmlToInput();
    },

    previewHtmlToInput() {
      var that = this;
      var contentLinePreview = document.getElementById("contentLinePreview");
      that.editRow.sayContent = contentLinePreview.innerHTML;
    },
    formatHTML(strs) {
      //声明left变量用于存放html标签中左尖括号（‘<’）位置
      var left = null;
      //声明right变量用于存放html标签中右尖括号（‘<’）位置
      var right = null;
      //声明str变量，用于存放格式化后的代码字符串
      var str = "";
      //存放html代码所进所用的空格
      var blank = "  ";
      //存放若干个blank变量，用于控制代码缩进的深度
      var fmt = [];
      var isInTag = false;
      //对需要格式化的代码字符串进行遍历
      for (var i = 0; i < strs.length; i++) {
        var curStr = strs[i];

        // str += fmt.join("") + curStr;

        //记录一个缩进
        if (curStr == "<" && strs[i + 1] != "/") {
          str += "\n";
          str += curStr;

          // str = str.substring(0, i) + "\n" + curStr;
          //如果在标签内，插入缩进
          // if (isInTag) {
          //   fmt.push(blank);
          // } else {
          //   isInTag = true;
          // }
        } else if (curStr == ">") {
          //发现右尖括号后将其记录在right变量上
          str += curStr;
          // str += "\n";
        } else if (curStr == "<" && strs[i + 1] == "/") {
          str += "\n";
          str += curStr;
        } else {
          str += curStr;
        }
        // if (curStr == "/") {
        //   //出栈
        //   fmt.pop();
        //   //如果出完了，标识为不在标签内
        //   if (fmt.length == 0) {
        //     isInTag = false;
        //   }
        // }
      }
      return str;
      var noTag = "";
      for (var i = 0; i < strs.length; i++) {
        //发现左尖括号后将其位置记录在left变量上
        if (strs[i] == "<") {
          // str += "\n";
          left = i;
        } else if (strs[i] == ">") {
          //发现右尖括号后将其记录在right变量上
          right = i;
        }
        // 当做尖括号右尖括号都记录了一个位置后，说明二者之间的内容为代码的一行
        if (typeof left == "number" && typeof right == "number") {
          // str += noTag;
          // noTag = "";

          //判断字符串左尖括号后是否为‘/’，如果满足，表明该行代码为双标签的闭合标签
          if (strs[left + 1] == "/") {
            //对数组中的空格做出栈，确保代码缩进正确
            fmt.pop();
            //将该行代码放入str变量中
            str += fmt.join("") + strs.slice(left, right + 1);
            //判断字符串右尖括号前一位是否为‘/’，如满足，表明该标签为严格闭合的单标签
          } else if (strs[right - 1] == "/") {
            str += fmt.join("") + strs.slice(left, right + 1);
            //判断字符串开头是否包含input/imig/hr/br/link/meta等字母，用于屏蔽非严格未闭合的单标签
          } else if (
            strs
              .slice(left, right)
              .search(/\<input|\<img|\<hr|\<br|\<link|\<meta/) != -1
          ) {
            str += fmt.join("") + strs.slice(left, right + 1);
            //对双标签的左标签进行的操作
          } else {
            str += fmt.join("") + strs.slice(left, right + 1);
            //向数组中堆入一个空格，确保下一行双标签的左标签的缩进正确
            fmt.push(blank);
          }

          //对right位置后的字符串进行遍历
          // for (var j = right; j < strs.length; j++) {
          //   //查找right位置后，第一个左尖括号的位置，二者之间的内容即为代码中的文本内容
          //   if (strs[j] == "<") {
          //     //去掉文本中多余的空格
          //     var s = strs.slice(right + 1, j).replace(/\s*/g, "");
          //     if (s) {
          //       //当文本中去掉空格后任然有内容，则将文本拼入str变量进行存储
          //       str += s;
          //     }
          //     break;
          //   }
          // }
          //每次获得一次左右尖括号的位置后，即得到了一行代码，为代码做换行处理
          str += "\n";
          //重置left、right的值，用于for循环的下次存储做右尖括号的位置
          left = null;
          right = null;
        } else if (typeof left != "number" && typeof right != "number") {
          str += strs[i];
          // noTag += strs[i];
        }
      }
      //返回得到的格式化完成的html代码字符串
      return str;
    },
    addListener() {
      var that = this;
      //监听 contentLinePreview 变化

      that.$nextTick(() => {
        var contentLinePreview = document.getElementById("contentLinePreview");
        // document.onselectionchange = (e) => {

        // };
        contentLinePreview.addEventListener("keyup", (e) => {
          // that.editRow.sayContent = that.formatHTML(
          //   contentLinePreview.innerHTML
          // );
          that.editRow.sayContent = contentLinePreview.innerHTML;
        });
      });

      document.onkeyup = (e) => {
        switch (e.key) {
          case "Escape":
            //刚刚关闭
            if (that.isJustNowClose) {
              that.isJustNowClose = false;
            } else {
              that.handleClose();
            }
            break;
        }
      };

      document.onkeydown = (e) => {
        if ((e.ctrlKey || e.metaKey) && (e.key === "s"||e.key === "S")) {
          //  执行save方法
          // this.save();
          
          that.onSubmit();
          // 阻止默认事件
          e.preventDefault();
        }
      };
    },
    show(row) {
      var that = this;
      this.isShowDrawer = true;

      this.editRow = JSON.parse(JSON.stringify(row));

      this.dynamicTags = this.editRow.noteTags;
      this.oldContentVal = this.editRow.sayContent;
      that.divContent = this.editRow.sayContent;
      that.addListener();
    },
    // 保存
    onSubmit() {
      var that = this;

      that.$http.post("/api/Note/SaveNoteWithTag", that.editRow).then((res) => {
        // //关闭面板
        if (res.succeeded) {
          // that.isShowDrawer = false;
          that.$message.success("保存成功");
          that.editRow.sayContent = res.data.findNote.sayContent;
          that.oldContentVal = that.editRow.sayContent;
          that.$emit("RefreshData");
        }
      });
    },
    handleClose(done) {
      var that = this;

      //内容没有更改，直接关闭
      if (this.editRow.sayContent == this.oldContentVal) {
        // done();
        that.isShowDrawer = false;
        return;
      }
      // done();
      this.$confirm("内容变更,关闭将丢失更改,确认关闭？")
        .then((_) => {
          // done();
          that.isShowDrawer = false;
        })
        .catch((_) => {
          if (_ == "close") {
            that.isJustNowClose = true;
          }
        });
    },

    tabFunc() {
      this.insertInputTxt("myinput", "\t");
    },
    insertInputTxt(id, insertTxt) {
      var elInput = document.getElementById(id);
      var startPos = elInput.selectionStart;
      var endPos = elInput.selectionEnd;
      if (startPos === undefined || endPos === undefined) return;
      var txt = elInput.value;
      // var result =
      //   txt.substring(0, startPos) + insertTxt + txt.substring(endPos);
      var result = this.replaceStr2(txt, startPos, endPos, insertTxt);
      elInput.value = result;
      elInput.focus();
      elInput.selectionStart = startPos + insertTxt.length;
      elInput.selectionEnd = startPos + insertTxt.length;
      this.editRow.sayContent = result;
    },
    //退回 上一步内容
    onGoBackStep() {
      this.editRow.sayContent = this.oldContentVal;
    },
    //输入框失去焦点
    onContentInputBlur(e) {
      var srcEl = e.srcElement;
      this.iptBlurSelectObj.selectionStart = srcEl.selectionStart;
      this.iptBlurSelectObj.selectionEnd = srcEl.selectionEnd;
    },
    //替换指定位置字符串
    replaceStr2(str, startIndex, endIndex, repStr) {
      return str.substring(0, startIndex) + repStr + str.substring(endIndex);
    },
    //转超链接
    onConvertUrl() {
      var that = this;

      if (
        this.iptBlurSelectObj.selectionStart ==
        this.iptBlurSelectObj.selectionEnd
      ) {
        that.$message.info("未选中内容");
        return;
      }

      //获取选中内容
      var selectTxt = this.editRow.sayContent.substring(
        this.iptBlurSelectObj.selectionStart,
        this.iptBlurSelectObj.selectionEnd
      );
      var repTxt = "";
      //如果已包含target="_blank" 转回去
      if (selectTxt.indexOf('target="_blank"') != -1) {
        // var onlyUrl = selectTxt.match(/(?<=_blank">)\s*.+\s*(?=<\/a>)/);
        var onlyUrl = selectTxt.match(/(?<=href=\").+?(?=\")/);
        repTxt = onlyUrl[0];
      } else {
        repTxt = `<a href="${selectTxt.trim()}" target="_blank">${selectTxt}</a>`;
      }

      this.editRow.sayContent = this.replaceStr2(
        this.editRow.sayContent,
        this.iptBlurSelectObj.selectionStart,
        this.iptBlurSelectObj.selectionEnd,
        repTxt
      );
      //转完后，光标清零
      this.iptBlurSelectObj.selectionStart = 0;
      this.iptBlurSelectObj.selectionEnd = 0;
    },

    //向字符串指定下标插入字符串
    //index 插入的下标
    insertStr(str, index, insertStr) {
      return str.substring(0, index) + insertStr + str.substring(index);
    },
    onContentPaste(e) {
      var that = this;
      var file = e.clipboardData.files[0];
      var txtAreaEl = e.srcElement;
      var oriSelectionStart = txtAreaEl.selectionStart;
      // return
      //有文件、图片
      if (file) {
        let formData = new FormData();
        formData.append("file", file);
        // 阻止默认事件
        e.preventDefault();
        that.$http.post("/api/File/UploadImg", formData).then((res) => {
          if (res.succeeded) {
            var insertImgSrc = `<img src="${process.env.VUE_APP_BASE_API}${res.data.url}" alt="">`;
            txtAreaEl.value = that.insertStr(
              txtAreaEl.value,
              oriSelectionStart,
              insertImgSrc
            );
            txtAreaEl.selectionStart = oriSelectionStart + insertImgSrc.length;
            txtAreaEl.selectionEnd = txtAreaEl.selectionStart;
            that.editRow.sayContent = txtAreaEl.value;
          }
        });
      }
    },
    //关闭标签
    handleCloseTag(tagItem) {
      // this.dynamicTags.splice(this.dynamicTags.indexOf(tag), 1);
      this.dynamicTags.splice(
        this.dynamicTags.findIndex((x) => x == tagItem),
        1
      );
    },

    showInput() {
      this.inputVisible = true;
      this.$nextTick((_) => {
        this.$refs.saveTagInput.$refs.input.focus();
      });
    },
    //确认添加标签
    handleInputConfirm() {
      let inputValue = this.inputValue;
      if (inputValue) {
        this.dynamicTags.push({ tagName: inputValue });
      }
      this.inputVisible = false;
      this.inputValue = "";
    },
    //确认删除
    confirmDel() {
      var that = this;
      that.$http
        .post("/api/Note/DelAContent", { id: that.editRow.id })
        .then((res) => {
          //关闭面板
          if (res.succeeded) {
            that.$message.success("删除成功");
            that.isShowDrawer = false;
            that.$emit("RefreshData");
          }
        });
    },
  },
};
</script>
<style  >
.el-color-picker__panel {
  z-index: 9999 !important;
}
#myinput {
  background: black;
  color: white;
}
.el-message-box__wrapper {
  z-index: 9999 !important;
}
/* 可以设置不同的进入和离开动画 */
/* 设置持续时间和动画函数 */
.slide-fade-enter-active {
  transition: all 0.3s linear;
}
.slide-fade-leave-active {
  transition: all 0.3s cubic-bezier(1, 1, 1, 2);
}
.slide-fade-enter,
.slide-fade-leave-to {
  transform: translateX(-100%);
  opacity: 0;
}
.contentLinePreview {
  padding: 7px 34px;
  border: 1px solid #d5d5d5;
  overflow: auto;
  line-height: 26px;
  height: 80%;
}
.el-message {
  z-index: 9999 !important;
}
.leftPreview {
  position: fixed;
  left: 0;
  top: 0;
  background: #fefefe;
  width: 50%;
  height: 100%;
  /* overflow: scroll; */
  /* padding: 38px 0 0 23px; */
  /* transition: all 2s; */
  z-index: 9998;
}
.leftPreview img {
  width: 50%;
}
.el-tag + .el-tag {
  margin-left: 10px;
}
.button-new-tag {
  margin-left: 10px;
  height: 32px;
  line-height: 30px;
  padding-top: 0;
  padding-bottom: 0;
}
.input-new-tag {
  width: 90px;
  margin-left: 10px;
  vertical-align: bottom;
}
.drawerStyle {
  /* padding:0 1px; */

  padding: 14px 27px;
}
</style>
