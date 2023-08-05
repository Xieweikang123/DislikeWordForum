<template>
  <div>
    <div v-if="noteHisList.length > 0" style="margin:0;display: flex;    align-items: center;">
      历史版本:
      <!-- {{ noteHisList.length }}个  -->
      <el-slider style="    width: 82%;" v-model="sliderValue" :step="sliderStep" :max="sliderMax"
        :format-tooltip="formatTooltip" show-stops>
      </el-slider>
    </div>

    <div style="    margin: 0 0 7px;">
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

      <Editor id="wangEditor" ref="wangEditor" class="editor" style="height: 500px; overflow-y: hidden;"
        v-model="curItem.sayContent" :defaultConfig="editorConfig" :mode="mode" @onCreated="onCreated" />
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


var doc = document,
  docEl = document.documentElement,
  body = doc.body,
  win = window;
// var wangEditor = document.getElementById('wangEditor')
// console.log('wangEditor', wangEditor)
// win = document.getElementById('wangEditor')

var slider = doc.createElement('div'),
  sliderSize = doc.createElement('div'),
  controller = doc.createElement('div'),
  sliderContent = doc.createElement('iframe'),
  scale = 0.1,
  realScale = scale;
var curY; // Variable to store the initial Y position of the mouse


slider.className = 'slider';
sliderSize.className = 'slider__size';
controller.className = 'slider__controller';

sliderContent.className += ' slider__content';
sliderContent.style.transformOrigin = '0 0';

slider.appendChild(sliderSize);
slider.appendChild(controller);
slider.appendChild(sliderContent);
body.appendChild(slider);



////////////////////////////////////////

function getDimensions() {
  var bodyWidth = body.clientWidth,
    bodyRatio = body.clientHeight / bodyWidth,
    winRatio = win.innerHeight / win.innerWidth;

  slider.style.width = (scale * 100) + '%';
  slider.style.height = '500px'

  // Calculate the actual scale in case a max-width/min-width is set.
  realScale = slider.clientWidth / bodyWidth;

  // sliderSize.style.paddingTop = (bodyRatio * 100) + '%';
  controller.style.paddingTop = (winRatio * 100) + '%';

  sliderContent.style.transform = 'scale(' + realScale + ')';
  sliderContent.style.width = (100 / realScale) + '%';
  sliderContent.style.height = (100 / realScale) + '%';
}

getDimensions();
win.addEventListener('resize', getDimensions);
win.addEventListener('load', getDimensions);


////////////////////////////////////////
// Click & Drag Events


function pointerLeave(e) {
  e.preventDefault();
  console.log('pointerLeave 1111111')
  // mouseDown = false;
  curY = undefined; // Reset the initial Y position
  // if (e.target === body) { mouseDown = false; }
}



slider.addEventListener('mousedown', pointerDown);
// slider.addEventListener('touchdown', pointerDown);

document.addEventListener('mousemove', pointerMove);
// slider.addEventListener('touchmove', pointerMove);
document.addEventListener('mouseup', pointerLeave);
slider.addEventListener('mouseup', pointerLeave);
// slider.addEventListener('touchend', pointerReset);

// 鼠标离开minimap
// slider.addEventListener('mouseleave', pointerLeave);
// slider.addEventListener('touchleave', pointerLeave);
var curTransformY = 0

//鼠标点击
function pointerDown(e) {
  e.preventDefault();
  curY = e.clientY; // Store the initial Y position of the mouse

  curTransformY = e.offsetY - parseFloat(document.getElementsByClassName('slider__controller')[0].offsetHeight)/2

  formatcurTransformY()

  controller.style.transform = 'translate(' +
    '0px, ' +
    ((curTransformY)) + 'px)';

  console.log('pointerDown curY', curY)
  console.log('pointerDown e.offsetHeight', e.offsetY)
}

//处理超框问题
function formatcurTransformY() {
  //超框问题
  if (curTransformY < 0) {
    curTransformY = 0
  }
  //底部超框
  console.log('curTransformY', curTransformY)
  var sliderHeight = parseFloat(document.getElementsByClassName('slider')[0].style.height)
  var sliderControllerHeight = parseFloat(document.getElementsByClassName('slider__controller')[0].offsetHeight)
  var maxHeight = sliderHeight - sliderControllerHeight
  if (curTransformY > maxHeight) {
    curTransformY = maxHeight
  }

}


function pointerMove(e) {
  // console.log('pointerMove11', e)
  // console.log('pointerMove', e)
  if (curY == undefined) {
    return
  }
  e.preventDefault();
  // console.log('slider.scrollTop', slider)

  // var currentY = e.clientY; // Get the current Y position of the mouse
  var distanceY = e.clientY - curY; // Calculate the distance of downward mouse movement
  curTransformY += distanceY

  formatcurTransformY()


  controller.style.transform = 'translate(' +
    '0px, ' +
    ((curTransformY)) + 'px)';
  curY = e.clientY
  // curTransformY = distanceY
  // // curY=curTransformY
  // console.log('curTransformY:', curTransformY);

  // controller.style.transform = 'translate(' +
  //   '0px, ' +
  //   ((curTransformY)) + 'px)';

  // var wheel = document.getElementsByClassName('w-e-scroll')[0]
  // wheel.scrollTo(0, curTransformY)

  // controller.style.transform = 'translate(' +
  //   ((1 * realScale)) + 'px, ' +
  //   ((wheel.scrollTop * realScale)) + 'px)';

}



import { Editor, Toolbar } from '@wangeditor/editor-for-vue'
// import { IEditorConfig } from '@wangeditor/editor'

export default {
  components: {
    Editor, Toolbar
  },
  data() {
    return {
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
      editorConfig: { placeholder: '请输入内容...', MENU_CONF: {} },
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
    isDataLoad(val) {
      if (val) {
        this.$nextTick(() => {
          var wangEditor = document.getElementById('wangEditor')

          console.log('mm', wangEditor)
          console.dir(wangEditor)
          // console.log('mm11', this.$refs.wangEditor)
          var html = wangEditor.outerHTML
            .replace(/<script([\s\S]*?)>([\s\S]*?)<\/script>/gim, '');// Remove all scripts
          // var script = '<script>var slider=document.querySelector(".slider"); slider.parentNode.removeChild(slider);<' + '/script>';
          var script = '<script>document.addEventListener("mouseup", function(event) { event.preventDefault(); });>' + '/script>';
          html = html.replace('</body>', script + '</body>');
          // console.log('html',html)

          // style="height: 500px; overflow-y: hidden;"
          html = html.replace('style="height: 500px; overflow-y: hidden;"', '')
          // Must be appended to body to work.
          var iframeDoc = sliderContent.contentWindow.document;

          // iframeDoc.addEventListener("mouseup", () => { console.log('111') })
          iframeDoc.open();
          iframeDoc.write(html);
          iframeDoc.close();

          sliderContent.contentWindow.document.addEventListener('mouseup', pointerLeave);
          sliderContent.contentWindow.document.addEventListener('mousedown', pointerDown);
          sliderContent.contentWindow.document.addEventListener('mousemove', pointerMove);
          // sliderContent.contentEditable = false;
          // sliderContent.contentWindow.document.contentEditable=false
          // wangEditor.addEventListener('scroll', this.trackScroll)
          // this.$refs.wangEditor.addEventListener('scroll', this.trackScroll);

          console.log('this.editor', this.editor)
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
    trackScroll(e) {
      var wheel = document.getElementsByClassName('w-e-scroll')[0]
      var sliderHeight = parseFloat(document.getElementsByClassName('slider')[0].style.height)
      controller.style.transform = 'translate(' +
        0 + 'px, ' +
        ((wheel.scrollTop * (sliderHeight / wheel.scrollHeight))) + 'px)';
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

  },
};
</script>
<style>
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
  border: solid 1px #737373;

}


.slider {

  border: 1px solid black;
  position: fixed;
  overflow: hidden;

  right: 40px;
  /* left: 10px; */
  top: 164px;

  /*     
  top: 10px;
  left: 10px; */
  min-width: 20px;
  max-width: 60px;
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
  position: relative;
  z-index: 3;
  height: inherit;
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