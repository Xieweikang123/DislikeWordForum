<template>
  <div>
    <div class="minimap-container">

      <div class="content">
        Lorem ipsum dolor sit amet, consectetur adipiscing elit...
        重复添加会报错吗？

        当使用Vue实现缩略地图功能时，你可以按照以下步骤进行详细实现：

        创建Vue组件：

        在Vue项目中创建一个新的组件，可以命名为Minimap。
        在组件模板中定义缩略地图容器和缩略图的元素。
        根据需要，为缩略地图容器和缩略图添加样式。
        获取文件内容：
        <br>

        在组件的mounted钩子函数中，使用合适的方式获取文件内容。这可以包括通过API请求、读取本地文件等。确保将文件内容保存到Vue组件的数据属性中，例如fileContent。
        生成缩略图：
        <br>

        创建一个方法，例如generateMinimap，用于根据文件内容生成缩略图。
        在该方法中，根据文件内容使用算法或库来生成缩略图数据。你可以考虑使用Canvas API或其他图形库来处理缩略图的生成。
        使用生成的缩略图数据来渲染缩略图元素。你可以将缩略图作为图像或使用SVG等技术进行渲染。
        处理主编辑器滚动事件：
        <br>
        创建一个方法，例如handleEditorScroll，用于处理主编辑器的滚动事件。
        在该方法中，根据主编辑器的滚动位置计算出相应的缩略图滚动位置。
        使用DOM操作或相关库来更新缩略图的滚动位置，以实现主编辑器和缩略图之间的同步滚动。
        处理缩略图点击事件：
        <br>
        创建一个方法，例如handleMinimapClick，用于处理缩略图的点击事件。<br>
        在该方法中，获取点击事件的位置信息，例如鼠标点击的坐标。<br>
        根据位置信息计算出用户想要查看的文件位置。<br>
        使用DOM操作或相关库来更新主编辑器的滚动位置，以将焦点定位到用户感兴趣的位置。<br>
        注册事件监听和清理：<br>

        在组件的mounted钩子函数中，注册主编辑器滚动事件的监听器，以及缩略图点击事件的监听器。<br>
        在组件的beforeDestroy钩子函数中，清理事件监听器，以防止内存泄漏。<br>

        <br>
        如果为空，contains 会报错吗？

        直接Add重复元素和 先判断元素是否重复然后再决定是否添加，两者效率对比
      </div>
      <!-- <div class="minimap" ref="minimap" @click="handleMinimapClick">
      </div> -->
      <canvas id="myCanvas" width="500" height="300"></canvas>

    </div>

    <!--     
    <canvas id="tutorial" width="150" height="150">
      current stock price: $3.15 + 0.15
    </canvas> -->
  </div>
</template>

<script>
export default {
  data() {
    return {
      fileContent: '', // 文件内容
      thumbnail: '', // 缩略图数据/URL
      minimapHeight: 0 // 缩略图容器高度
    };
  },
  mounted() {

    const canvas = document.getElementById('myCanvas');
    const context = canvas.getContext('2d');


    context.fillStyle = "rgb(200, 0, 0)";
    context.fillRect(10, 10, 50, 50);

    context.fillStyle = "rgba(0, 0, 200, 0.5)";
    context.fillRect(30, 30, 50, 50);

    context.fillStyle = "rgba(0, 0, 200, 0.5)";
    context.fillRect(50, 50, 50, 50);

    // 绘制矩形
    // context.fillStyle = 'red'; // 设置填充颜色
    // context.fillRect(50, 50, 200, 100); // 绘制一个宽度为200，高度为100的矩形

    // 绘制文本
    context.font = '24px Arial'; // 设置字体样式
    context.fillStyle = 'blue'; // 设置填充颜色
    context.fillText('Hello, Canvas!', 100, 200); // 在坐标(100, 200)处绘制文本


    // // 获取文件内容并生成缩略图
    // this.getFileContent().then(() => {
    //   this.generateThumbnail();
    // });

    // // 监听主编辑器滚动事件，更新缩略图
    // this.$refs.minimap.addEventListener('scroll', this.handleEditorScroll);
  },
  beforeDestroy() {
    // 移除事件监听
    this.$refs.minimap.removeEventListener('scroll', this.handleEditorScroll);
  },
  methods: {

    generateThumbnail() {
      // 模拟生成缩略图的操作
      // 这里使用简单的文本截取作为缩略图的演示
      const maxLength = 50;
      if (this.fileContent.length > maxLength) {
        this.thumbnail = this.fileContent.substring(0, maxLength) + '...';
      } else {
        this.thumbnail = this.fileContent;
      }

      // 设置缩略图容器的高度，适应文件内容的比例
      const lineHeight = 16; // 假设每行高度为16px
      this.minimapHeight = Math.min(this.fileContent.length * lineHeight, 200); // 最大高度为200px
    },

    getFileContent() {
      // 模拟获取文件内容的异步操作
      return new Promise((resolve) => {
        setTimeout(() => {
          this.fileContent = 'Lorem ipsum dolor sit amet, consectetur adipiscing elit...';
          resolve();
        }, 1000);
      });
    },
    generateThumbnail() {
      // 模拟生成缩略图的操作
      // 这里使用简单的文本截取作为缩略图的演示
      const maxLength = 50;
      if (this.fileContent.length > maxLength) {
        this.thumbnail = this.fileContent.substring(0, maxLength) + '...';
      } else {
        this.thumbnail = this.fileContent;
      }

      // 设置缩略图容器的高度，适应文件内容的比例
      const lineHeight = 16; // 假设每行高度为16px
      this.minimapHeight = Math.min(this.fileContent.length * lineHeight, 200); // 最大高度为200px
    },
    handleEditorScroll() {
      // 处理主编辑器滚动事件 
      // 这里仅作演示，直接将缩略图滚动到相应位置
      // const editorScrollTop = this.$refs.editor.scrollTop;
      // const minimapScrollTop = editorScrollTop * this.minimapHeight / this.$refs.editor.scrollHeight;
      // this.$refs.minimap.scrollTop = minimapScrollTop;
    },
    handleMinimapClick(event) {
      // 处理缩略图点击事件
      // 这里仅作演示，将主编辑器的滚动位置定位到点击位置
      const minimapClickY = event.clientY - this.$refs.minimap.getBoundingClientRect().top;
      const editorScrollTop = minimapClickY * this.$refs.editor.scrollHeight / this.minimapHeight;
      this.$refs.editor.scrollTop = editorScrollTop;
    }
  }
};
</script>

<style>
.minimap-container {
  display: flex;
  height: 300px;
  overflow: auto;
  /* 设置缩略图容器的高度 */
}

.content {
  width: 80%;
}

.minimap {

  width: 20%;
  /*   
  position: absolute;
  top: 0;
  right: 0;
  overflow: auto; */
  /* 设置缩略图容器的样式，如宽度、背景色等 */
}

img {
  width: 100%;
  height: auto;
}
</style>