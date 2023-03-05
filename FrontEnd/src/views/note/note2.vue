<template>
  <div>
    <div v-if="isDataLoad" style="border: 1px solid #ccc;">
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
      isDataLoad: false,
      curItem: {},
      editor: null,
      // html: '<p>hello</p>',
      toolbarConfig: {},
      editorConfig: { placeholder: '请输入内容...', MENU_CONF: {} },
      mode: 'default', // or 'simple'

    };
  },
  computed: {
    //当前标签
    currentTagName() {
      return this.pageInfo.searchKeyValues[0].value;
    },
  },
  mounted() {
    var that = this;
    that.registerHotKey()
    that.imgConfig()
    that.getCurData()
  },
  beforeDestroy() {
    const editor = this.editor
    if (editor == null) return
    editor.destroy() // 组件销毁时，及时销毁编辑器
  },
  methods: {
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

      that.$http
        .post("/api/Note/GetNoteById", { id: curNoteId })
        .then((res) => {
          console.log("GetNoteById", res);
          // that.noteHisList = res.data;
          if (res.succeeded) {
            that.isDataLoad = true
            that.curItem = res.data
            // document.title = "笔记-回收站"
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
          console.log('customUpload', file)
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
    // 保存
    onSubmit() {
      console.log('submit', this.curItem)
      var that = this;
      console.log(' window.opener', window.opener)

      that.$http.post("/api/Note/SaveNoteWithTag", that.curItem).then((res) => {
        // //关闭面板
        if (res.succeeded) {
          // that.isShowDrawer = false;
          that.$message.success("保存成功");
          console.log('inter', window.opener)
          console.log('inter aa', window.opener.pGetNoteList)
          window.opener.pGetNoteList()
          // window.opener.parentMethod()
        } else {
          that.$message.error("保存出错");
        }
      });
    },
    onCreated(editor) {
      this.editor = Object.seal(editor) // 一定要用 Object.seal() ，否则会报错
    },
  },
};
</script>
  
<style >
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