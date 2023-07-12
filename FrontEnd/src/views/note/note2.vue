<template>
  <div>
    <div v-if="noteHisList.length > 0" style="margin: 18px 31px">
      历史版本: {{ noteHisList.length }}个
      <el-slider v-model="sliderValue" :step="sliderStep" :max="sliderMax" :format-tooltip="formatTooltip" show-stops>
      </el-slider>
    </div>

    <div>
      <el-tag :key="titem.id" v-for="titem in dynamicTags" closable :disable-transitions="false"
        @close="handleCloseTag(titem)">
        {{ titem.tagName }}
        <!-- {{ titem }} -->
      </el-tag>
      <el-input class="input-new-tag" v-if="inputVisible" v-model="inputValue" ref="saveTagInput" size="small"
        @keyup.enter.native="handleInputConfirm" @blur="handleInputConfirm">
      </el-input>
      <el-button v-else class="button-new-tag" size="small" @click="showInput">+ New Tag</el-button>
    </div>
    <div v-if="isDataLoad || isAdd" style="border: 1px solid #ccc;">
      <Toolbar style="border-bottom: 1px solid #ccc" :editor="editor" :defaultConfig="toolbarConfig" :mode="mode" />
      <Editor style="height: 500px; overflow-y: hidden;" v-model="curItem.sayContent" :defaultConfig="editorConfig"
        :mode="mode" @onCreated="onCreated" />
      <el-button type="primary" @click="onSubmit">保存</el-button>
    </div>
    <div v-else>
      <el-skeleton :rows="20" animated style="margin-top: 10px" />
      <!-- 数据加载中... -->
    </div>
  </div>
</template>
<style src="@wangeditor/editor/dist/css/style.css"></style>

<script>
import { Editor, Toolbar } from '@wangeditor/editor-for-vue'
// import { IEditorConfig } from '@wangeditor/editor'

export default {
  components: {
    Editor, Toolbar
  },
  data() {
    return {
      dynamicTags: [{ tagName: '标签1' }],
      inputVisible: false,
      sliderMax: 1,
      sliderStep: 1,
      noteHisList: [],
      inputValue: '',
      lastSliderValue: -1,
      sliderValue: 100,
      initContent: '',
      isDataLoad: false,
      curItem: { id: '' },
      editor: null,
      // html: '<p>hello</p>',
      toolbarConfig: {},
      editorConfig: { placeholder: '请输入内容...', MENU_CONF: {} },
      mode: 'default', // or 'simple'

    };
  },
  computed: {
    //是否新增
    isAdd() {
      return this.$route.query.id.length == 0
    },
  },
  watch: {
    noteHisList: {
      handler(nVal) {
        this.sliderMax = nVal.length;
        this.sliderValue = this.sliderMax;
      },
    },
  },
  mounted() {
    var that = this;
    that.registerHotKey()
    that.imgConfig()
    console.log('ll', this.$route.query.id)
    //修改
    if (!this.isAdd) {
      that.getCurData()
      //获取笔记历史
      that.getCurNoteHis();
    } else {
      this.dynamicTags = []
      //新增
      // dynamicTags: [{ tagName: '标签1' }],
      if (this.$route.query.tagName) {
        this.dynamicTags = [{ tagName: this.$route.query.tagName }]
      }
    }

    window.onbeforeunload = function () {
      //如果内容变了，询问是否关闭，如果内容没变，返回null
      if (that.initContent == that.curItem.sayContent) {
        return null
      }
      return '您有未保存的更改，确定要离开吗？';
    };
  },
  beforeDestroy() {
    const editor = this.editor
    if (editor == null) return
    editor.destroy() // 组件销毁时，及时销毁编辑器
  },
  methods: {
    // 保存
    onSubmit() {
      var that = this;
      that.curItem.noteTags = this.dynamicTags
      that.$http.post("/api/Note/SaveNoteWithTag", that.curItem).then((res) => {
        // //关闭面板
        if (res.succeeded) {
          that.$message.success("保存成功");
          //如果是新增，跳转
          if (this.isAdd) {
            //跳转不提示是否保存
            window.onbeforeunload = null
            window.location.href = '/note2?id=' + res.data.dto.id
          }
          that.initContent = that.curItem.sayContent
          //保存之后重新获取历史笔记
          that.getCurNoteHis();
          window.opener.pGetNoteList()
        } else {
          that.$message.error("保存出错");
        }
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
    //获取当前笔记对应的所有历史笔记
    getCurNoteHis() {
      var that = this;
      that.$http
        .post("/api/Note/GetCurNoteHisList", { id: this.$route.query.id })
        .then((res) => {

          that.noteHisList = res.data;
        });
    },
    //历史版本滑动条文本格式化显示
    formatTooltip(value) {
      //value 没变的话，也不往下

      // 如果本次value和上次一样，不往下进行
      if (value == null || value == this.lastSliderValue) {
        return;
      }
      this.lastSliderValue = value;
      this.curItem.sayContent = "";
      console.log('format')
      if (value == this.noteHisList.length) {
        //切换内容到当前内容
        this.setTwoContent(this.initContent);
        return "当前版本";
      }
      var slideItem = this.noteHisList[value];
      var time = slideItem.createTime.replace("T", " ");
      this.setTwoContent(slideItem.sayContent);
      return time;
    },
    //设置两个控件的值
    setTwoContent(content) {
      // this.editRow.sayContent = content;
      // this.divContent = content;
      this.curItem.sayContent = content
    },
    // 注册热键
    registerHotKey() {
      var that = this;
      //保存
      document.onkeydown = (e) => {
        if ((e.ctrlKey || e.metaKey) && (e.key === "s" || e.key === "S")) {
          that.onSubmit();
          // 阻止默认事件
          e.preventDefault();
        }
      };
    },
    //获取当前笔记
    getCurData() {
      var that = this
      var curNoteId = this.$route.query.id
      // that.isDataLoad = false
      that.$http
        .post("/api/Note/GetNoteById", { id: curNoteId })
        .then((res) => {
          if (res.succeeded) {
            that.isDataLoad = true
            that.curItem = res.data
            that.initContent = that.curItem.sayContent
            this.dynamicTags = this.curItem.noteTags;
            document.title = that.curItem.sayContent.replace(/(<([^>]+)>)/gi, "").substr(0, 20)
          } else {
            that.$message.error("数据加载失败");
          }
        });


    },
    //配置图片上传
    imgConfig() {
      var that = this;
      that.editorConfig.MENU_CONF['uploadImage'] = {
        fieldName: 'file',
        customUpload(file, insertFn) {

          let formData = new FormData();
          formData.append("file", file);
          that.$http.post("/api/File/UploadImg", formData).then((res) => {
            if (res.succeeded) {
              var src = `${process.env.VUE_APP_BASE_API}${res.data.url}`
              insertFn(src, 'img', src)
            }
          });
        },
      }
    },

    onCreated(editor) {
      this.editor = Object.seal(editor) // 一定要用 Object.seal() ，否则会报错
    },
  },
};
</script>
  
<style scoped>
.openMore {
  width: 100%;
  text-align: center;
  color: #205924;
  font-weight: bold;
  font-size: 32px;
  margin-top: -37px;
  cursor: pointer;
}

.contentLine {
  /* position: relative; */
  padding: 14px 40px;
  line-height: 26px;
}

.contentLine img {
  width: 50%;
}

/* .noteMask::after {
  content: "展开";
  position: absolute;
  bottom: 20px;
  color: red;
  font-size: 22px;
  width: 100%;
  background: #181859;
} */
.noteMask {
  max-height: 600px;
  overflow-y: hidden;
  background-image: linear-gradient(180deg,
      rgb(255 255 255 / 0%) 50%,
      #b0b0b0 100%);
}

.noteItemCls {
  border-bottom: 1px solid #e8e6e6;
  padding: 24px 0px;
}

.contentLine img {
  cursor: cell;
}

.medium-zoom-overlay {
  position: fixed;
  top: 0;
  right: 0;
  bottom: 0;
  left: 0;
  opacity: 0.8;
  transition: opacity 300ms;
  will-change: opacity;
  background: rgba(0, 0, 0, 0.8);
  z-index: 2;
}

.imgScale {
  position: absolute;
  transform: scale(1.5);
  z-index: 3;
}

/* .imgScale>div{
  width:200px
} */
img {
  transition: all 500ms cubic-bezier(0.2, 0, 0.2, 1);
}

.tagAllStyle {
  background-color: #447154 !important;
  border-color: #e1f3d8;
  color: #ffffff !important;
}


.topTagContainer {
  margin-bottom: 15px;
  display: flex;
  align-items: center;
}

.contentInput img {
  width: 100px;
}

.contentInput {
  min-height: 150px;
  height: auto;
  border: 1px solid #dadada;
  padding: 2px 4px;
}

.tagItemStyle {
  color: #363f4e;
  margin: 6px 2px;
  cursor: pointer;
}

.leftTagContainer {
  width: 16%;
  position: absolute;
}

.userHead:hover {
  opacity: 1;
}

.userHead {
  opacity: 90%;
  /* margin-top: 12px; */
  transition: opacity 0.7s;
}
</style>