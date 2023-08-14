<template>
  <div>
    <div v-if="noteHisList.length > 0" style="margin:0;display: flex;    align-items: center;">
      历史版本:
      <!-- {{ noteHisList.length }}个  -->
      <el-slider style="    width: 82%;" v-model="sliderValue" :step="sliderStep" :max="sliderMax"
        :format-tooltip="formatTooltip" show-stops>
      </el-slider>
    </div>

    <div style=" margin: 0 0 7px;">
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
      <div class="split-pane-wrapper" @mousemove="mouseMoveTrigger">
        <div class="leftMenu pane-left" :style="{ width: leftOffset + 'px' }">
          <ul id="header-container"></ul>
          <!-- 标题导航 -->
        </div>
        <div class="pane-trigger-con" @mousedown="mouseDownTrigger"></div>
        <div class="pane-right">
          <Editor id="wangEditor" ref="wangEditor" class="editor" style="height: 500px;     flex: 1;overflow-y: hidden;"
            v-model="curItem.sayContent" :defaultConfig="editorConfig" @onChange="onChange" :mode="mode"
            @onCreated="onCreated" />

          <!-- <div id="slider"></div> -->
        </div>
      </div>
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

var win = window;
var slider,
  sliderSize = document.createElement('div'),
  controller = document.createElement('div'),
  sliderContentIframe = document.createElement('iframe'),
  scale = 0.1,
  realScale = scale;
var curY; // Variable to store the initial Y position of the mouse
var sliderEditorHeight = 0;

// 标题 DOM 容器
var headerContainer;

////////////////////////////////////////
function getDimensions() {
  // var bodyWidth = body.clientWidth,
  //   bodyRatio = body.clientHeight / bodyWidth,
  var winRatio = win.innerHeight / win.innerWidth;

  slider.style.width = '60px'
  // slider.style.height = '500px'

  // Calculate the actual scale in case a max-width/min-width is set.

  controller.style.paddingTop = (winRatio * 100) + '%';

  // sliderContentIframe.style.transform = 'scale(' + realScale + ')';
  // sliderContentIframe.style.width = (100 / realScale) + '%';


}
win.addEventListener('resize', getDimensions);


////////////////////////////////////////
// Click & Drag Events


function pointerLeave(e) {
  curY = undefined; // Reset the initial Y position
}


document.addEventListener('mousemove', pointerMove);
document.addEventListener('mouseup', pointerLeave);

var curTransformY = 0

//鼠标点击
function pointerDown(e) {
  curY = e.clientY; // Store the initial Y position of the mouse
  curTransformY = e.offsetY - parseFloat(document.getElementsByClassName('slider__controller')[0].offsetHeight) / 2
  formatcurTransformY()

  controller.style.transform = 'translate(' +
    '0px, ' +
    ((curTransformY)) + 'px)';

  document.getElementsByClassName('w-e-scroll')[0].scrollTo(0, geteditorScrollYDis())
}

//处理超框问题
function formatcurTransformY() {
  //超框问题
  if (curTransformY < 0) {
    curTransformY = 0
  }
  //底部超框
  var sliderHeight = parseFloat(document.getElementsByClassName('slider')[0].style.height)
  //选框 高度
  var sliderControllerHeight = parseFloat(document.getElementsByClassName('slider__controller')[0].offsetHeight)
  var maxHeight = sliderEditorHeight * realScale - sliderControllerHeight / 2

  if (curTransformY > maxHeight) {
    curTransformY = maxHeight
  }
}

//编辑器应该移动的距离
function geteditorScrollYDis() {
  //编辑器滚动距离=滚动条top/(滚动条高度/编辑器高度)
  var editorScrollYDis = curTransformY / wheelSliderScale()
  return editorScrollYDis
}
//slider和编辑器高度比例
function wheelSliderScale() {
  var wheel = document.getElementsByClassName('w-e-scroll')[0]
  return (sliderEditorHeight * realScale) / wheel.scrollHeight
}
function pointerMove(e) {
  if (curY == undefined) {
    return
  }
  e.preventDefault();
  var distanceY = e.clientY - curY; // Calculate the distance of downward mouse movement
  curTransformY += distanceY
  formatcurTransformY()
  document.getElementsByClassName('w-e-scroll')[0].scrollTo(0, geteditorScrollYDis())
  controller.style.transform = 'translate(' +
    '0px, ' +
    ((curTransformY)) + 'px)';
  curY = e.clientY
}



import { Editor, Toolbar } from '@wangeditor/editor-for-vue'
// import { IEditorConfig } from '@wangeditor/editor'

export default {
  components: {
    Editor, Toolbar
  },
  data() {
    return {
      leftOffset: 200,
      triggerDragging: false,
      hList: [],
      editorHeight: 0,
      editorScrollHeight: 0,
      minimapHeight: 0,
      minimapThumbHeight: 0,
      thumbRatio: 0,
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
      editorConfig: {
        placeholder: '请输入内容...', MENU_CONF: {},

      },
      mode: 'default', // or 'simple'

    };
  },
  computed: {
    minimapStyle() {
      return {
        background: `url(${this.minimapImageUrl})`,
        backgroundSize: 'cover'
      }
    },
    minimapImageUrl() {
      const editor = this.$refs.editor
      const rect = editor.getBoundingClientRect()
      const width = rect.width
      const height = Math.floor(this.editorHeight / this.editorScrollHeight * this.minimapHeight)
      const top = Math.floor(-editor.scrollTop / this.editorScrollHeight * this.minimapHeight)
      return `element(#${editor.id}, rect(${rect.left}px, ${rect.top}px, ${rect.right}px, ${rect.bottom}px), ${width}px, ${height}px)`;
    },
    //是否新增
    isAdd() {
      return this.$route.query.id.length == 0
    },
  },
  watch: {
    'curItem.sayContent': {
      handler(nval) {
        var regex = /<h\d>.*?<\/h\d>/g
        this.hList = nval.match(regex)
        // this.reloadIframe()
      },
      deep: true
    },
    isDataLoad(val) {
      if (val) {
        this.$nextTick(() => {
          // slider = document.getElementById('slider')
          // slider.addEventListener('mousedown', pointerDown);
          // slider.addEventListener('mouseup', pointerLeave);

          // slider.className = 'slider';
          // sliderSize.className = 'slider__size';
          // controller.className = 'slider__controller';

          // sliderContentIframe.className += ' slider__content';
          // sliderContentIframe.style.transformOrigin = '0 0';

          // slider.appendChild(sliderSize);
          // slider.appendChild(controller);
          // slider.appendChild(sliderContentIframe);
          headerContainer = document.getElementById('header-container')
          headerContainer.addEventListener('mousedown', event => {
            if (event.target.tagName !== 'LI') return
            event.preventDefault()
            const id = event.target.id.slice(0, -1)
            this.editor.scrollToElem(id) // 滚动到标题
          })


          // var wheel = document.getElementsByClassName('w-e-scroll')[0]
          // this.reloadIframe()
          // getDimensions()
          // realScale = slider.clientWidth / wheel.clientWidth
          // console.log('aaa realScale', realScale)
          // console.dir(sliderContentIframe.contentDocument.body)
          // sliderContentIframe.contentDocument.body.style.transform = 'scale(' + realScale + ')';
          // sliderContentIframe.style.height = wheel.scrollHeight + 'px'

          // sliderContentIframe.contentWindow.document.addEventListener('mouseup', pointerLeave);
          // sliderContentIframe.contentWindow.document.addEventListener('mousedown', pointerDown);
          // sliderContentIframe.contentWindow.document.addEventListener('mousemove', pointerMove);


        })

      }
    },
    noteHisList: {
      handler(nVal) {
        this.sliderMax = nVal.length;
        this.sliderValue = this.sliderMax;
      },
    },
  },

  mounted() {
    var that = this;
    // leftOffset
    var leftOffset = localStorage.getItem('leftOffset')
    if (leftOffset) {
      this.leftOffset = leftOffset
    }

    that.registerHotKey()
    //配置图片上传
    that.imgConfig()
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

      //缓存拖拽距离
      localStorage.setItem('leftOffset', that.leftOffset)

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
    mouseMoveTrigger(event) {
      if (!event.which) {
        this.triggerDragging = false;
        document.body.style.cursor = "auto";
      }
      // 
      if (this.triggerDragging) {
        // 阻止默认的文本选择行为
        event.preventDefault();

        this.leftOffset = event.clientX - 20;
      }
    },
    mouseDownTrigger(event) {
      this.triggerDragging = true;
      document.body.style.cursor = "ew-resize";

    },
    onChange(editor) {

      var that = this
      const headers = editor.getElemsByTypePrefix('header')

      headerContainer.innerHTML = headers.map(header => {
        // const text = E.SlateNode.string(header)
        // const text = that.editor.SlateNode.string(header)
        const text = header.children[0].text
        const { id, type } = header
        return `<li id="${id}x" type="${type}">${text}</li>`
      }).join('')
    },
    getsliderEditorHeight() {
      sliderEditorHeight = 0
      if (!sliderContentIframe.contentDocument) {
        return
      }
      var wangEditorElement = sliderContentIframe.contentDocument.querySelector('#wangEditor');
      if (wangEditorElement) {
        console.dir(wangEditorElement)
        sliderEditorHeight = wangEditorElement.offsetHeight
        console.log('sliderEditorHeight', sliderEditorHeight)
      }
    },
    //重新渲染iframe内容
    reloadIframe() {
      // return
      var wangEditor = document.getElementById('wangEditor')
      this.getsliderEditorHeight()
      if (!wangEditor) {
        return
      }
      var html = wangEditor.outerHTML
        .replace(/<script([\s\S]*?)>([\s\S]*?)<\/script>/gim, '');// Remove all scripts
      var script = '<script>document.addEventListener("mouseup", function(event) { event.preventDefault(); });>' + '/script>';
      html = html.replace('</body>', script + '</body>');
      // html = html.replace('style="height: 500px; width: 100%; overflow-y: hidden;"', '')
      html = html.replace(/(?<=data-w-e-textarea="true").*?(?=>)/g, '')
      // Must be appended to body to work.
      var iframeDoc = sliderContentIframe.contentWindow.document;

      iframeDoc.open();
      iframeDoc.write(html);
      iframeDoc.close();
    },
    trackScroll(e) {
      var wheel = document.getElementsByClassName('w-e-scroll')[0]
      // var sliderHeight = parseFloat(document.getElementsByClassName('slider')[0].style.height)
      //iframe 也跟着滚动
      // console.log('sliderContentIframe', sliderContentIframe)
      console.dir(sliderContentIframe)
      sliderContentIframe.style.top = -wheel.scrollTop + 'px'


      //编辑器滚动距离顶部的距离* 缩略图与编辑器的比例
      controller.style.transform = 'translate(' + 0 + 'px, ' + ((wheel.scrollTop * realScale)) + 'px)';
    },
    onCreated(editor) {
      this.editor = Object.seal(editor) // 一定要用 Object.seal() ，否则会报错
      this.editor.on('scroll', this.trackScroll)
    },
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
      this.$set(this.curItem, 'sayContent', content)
      // this.curItem.sayContent = content
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

  },
};
</script>
<style>
:root {
  --editorHeight: 600px;
}

.split-pane-wrapper {
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: row;
}

.pane-right {
  flex: 1;
  overflow: auto;
  display: flex;
  /* background: chartreuse; */
}

.pane-trigger-con:hover {
  background: #71b8f1;
}

.pane-trigger-con {
  width: 15px;
  transition: background-color 0.3s ease-in-out;
  background: #f2f2f2;
  /* background: red; */
  cursor: ew-resize;
}

.leftMenu {
  border-right: 1px dashed rgb(180, 180, 180);
  height: var(--editorHeight);
  overflow-y: auto;
  /* min-width: 150px;
  max-width: 20%; */
}

#wangEditor {
  border-bottom: 1px solid #c8c8c8;
  height: var(--editorHeight) !important;
}

#header-container {
  list-style-type: none;
  padding-left: 0px;
  /* width: max-content; */
}

#header-container li {
  color: #333;
  margin: 10px 0;
  cursor: pointer;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

#header-container li:hover {
  text-decoration: underline;
}

#header-container li[type="header1"] {
  font-size: 20px;
  font-weight: bold;
}

#header-container li[type="header2"] {
  font-size: 16px;
  padding-left: 15px;
  font-weight: bold;
}

#header-container li[type="header3"] {
  font-size: 14px;
  padding-left: 30px;
}

#header-container li[type="header4"] {
  font-size: 12px;
  padding-left: 45px;
}

#header-container li[type="header5"] {
  font-size: 12px;
  padding-left: 60px;
}

/* 滚动条整体样式 */
::-webkit-scrollbar {
  width: 10px;
  /* 滚动条宽度 */
}

/* 滚动条轨道 */
::-webkit-scrollbar-track {
  background-color: #f1f1f1;
  /* 轨道背景色 */
}

/* 滚动条滑块 */
::-webkit-scrollbar-thumb {
  background-color: #939393;
  border-radius: 3px;
}

/* 鼠标悬停在滚动条上时的滑块样式 */
::-webkit-scrollbar-thumb:hover {
  background-color: #555;
  /* 悬停时滑块背景色 */
}

.slider__controller {
  width: 100%;
  padding-top: 100%;
  position: absolute;
  transition: transform 0.1s ease;
  background: #dae3ed40;
  top: 0;
  left: 0;
  transform-origin: 0 0;
  border-radius: 2px;
  box-shadow: inset 0 0 19px 5px #4b4b4b54;
  /* border: solid 1px #737373; */
}


.slider {

  border: 1px solid black;
  position: relative;
  overflow: hidden;
  /* min-width: 20px;
  max-width: 60px; */
  box-shadow: 0 2px 13px rgba(0, 0, 0, 0.3);
  cursor: grab;
  opacity: 0.5;
  transition: opacity 800ms ease-in-out 200ms;
  z-index: 999;

  &:hover {
    opacity: 1;
    transition-delay: 0ms;
  }
}

.slider__size {
  /* position: relative; */
  height: 100%;
  z-index: 3;
  /* height: inherit; */
}

.slider__content {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: -1;
  transform-origin: 0 0;
}



.content {
  max-width: 26em;
  padding: 2em;
  margin: auto;
}

p {
  margin: 0 0 2em;
}

/* 
img {
  max-width: 100%;
  height: auto;
  margin-bottom: 2em;
} */
/* 
img {
  display: block;
  position: relative;
  left: -50vw;
  margin-left: 50%;

  width: 100vw;
  max-width: none;
} */
</style>
<style scoped>
.w-e-full-screen-container {
  z-index: 1002;
}

/* 
::v-deep .el-slider__button-wrapper {
  z-index: 10 !important;
} */

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