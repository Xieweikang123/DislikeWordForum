<template>
  <div>
    <div style="text-align: center;">
      当前连接:
      <el-select v-model="curConfig.curSelect" placeholder="请选择">
        <el-option v-for="item in options" :key="item.value" :label="item.label" :value="item.value">
        </el-option>
      </el-select>
      <el-button type="primary" @click="opOpenDbConfig()">配置</el-button>
    </div>

    <el-button type="primary" @click="getAllDbs()">查数据库</el-button>


    <div class="dbContainer">
      <!-- <el-button v-for="item in dbList">{{ item }}</el-button> -->
      <div class="dbdiv" :class="{ 'fontRed': itemDbName == curConfig.curDbName }" v-for=" itemDbName in dbList">
        <span @click="dbNameClick(itemDbName)"> {{ itemDbName }} </span>
        <div ref="box" class="box" v-if="itemDbName == curConfig.curDbName">

          <!-- 数据库列表 -->
          <div class="tableSty left">
            <div class="tbNameItem" :class="{ 'fontRed': curTableName == item.name }" v-for="item in tableList"
              @click.stop="tableNameClick(item)">
              {{ item.name }}<span v-if="item.description"> ({{ item.description }})</span>
            </div>
          </div>
          <div class="resize" title="收缩侧边栏">

          </div>
          <div class="mid">
            <div style="    margin-top: -37px;margin-bottom: 16px;">
              <el-button size="small" @click="clickNameSql(item)" v-for="item in myNameSqls" type="primary"
                :plain="item.sqlName != curConfig.sqlName">{{ item.sqlName
                }}</el-button>
            </div>
            <!-- <div>myNameSqls</div> -->
            <div style="display: flex;">
              <el-input v-model="curConfig.sqlName" style="    width: 266px;    margin-right: 17px;"></el-input>
              <el-button :loading="loading" style="margin-bottom: 5px;" @click="saveSql()">保存</el-button>
              <el-button :loading="loading" style="margin-bottom: 5px;" @click="saveNewSql()">新增</el-button>

            </div>
            <el-button :loading="loading" style="margin-bottom: 5px;" @click="getSelected()">▶️执行</el-button>

            <CommonEditor ref="editor" :value="curConfig.code" language="sql"
              @change="editorChange" :tableNames="tableList" style=" width: 83%;height: 200px">
            </CommonEditor>
            <span v-if="isExecuted">共 {{ tableData.length }}</span>
            <!-- {{ curTableName }} -->
            <el-table row-class-name="table-row" stripe v-loading="loading" :row-style="{ height: '80px' }"
              :data="tableData" style="width: 100%">
              <!-- <el-table-column show-overflow-tooltip prop="id" label="id" width="180">
              </el-table-column> -->
              <el-table-column sortable v-for="item in tableColumns" show-overflow-tooltip :prop="item.dbColumnName"
                :label="item.dbColumnName" width="180">
              </el-table-column>
            </el-table>
          </div>
        </div>
      </div>

    </div>
    <dbConfigForm @loadMyDbs="loadMyDbs" :dbConfigData="dbConfigData" ref="dbConfigForm"></dbConfigForm>
  </div>
</template>
<script>
import dbConfigForm from "./dbConfigForm"
import CommonEditor from '@/components/CommonEditor.vue'

export default {
  components: {
    CommonEditor,
    dbConfigForm
  },
  data() {
    return {
      isExecuted: false,
      loading: false,
      tableData: [],
      tableColumns: [],
      curTableName: '',
      tableList: [],
      dbList: [],
      activeTab: '0',
      dbForm: {
        dbType: 'mssql',
        dbName: 'master'
      },
      dbConfigData: [],
      dialogVisible: false,
      options: [],
      myNameSqls: [],
      curConfig: {
        dbSqlId: '',
        sqlName: '',
        code: '',
        curDbName: '',
        curSelect: ''
      }

    };
  },
  computed: {
    //数据库选项下标
    currentSelectDbIndex() {
      return this.options.findIndex(x => x.value == this.curConfig.curSelect)
    }
  },
  watch: {
    curConfig: {
      handler: function (newVal, oldVal) {

        localStorage.setItem('curConfig', JSON.stringify(this.curConfig))
      },
      deep: true
    },
    //数据库改变
    'curConfig.curDbName'() {
      //获取表
      this.getTables()
    },
    //当前选择
    'curConfig.curSelect'() {
      // localStorage.setItem('curSelect', this.curConfig.curSelect)

      // this.clearDb();
      //查所有数据库
      this.getAllDbs()
    },
  },
  mounted() {
    // 添加全局键盘事件监听器
    document.addEventListener('keydown', this.handleKeyDown);
    var configCache = localStorage.getItem('curConfig')
    if (configCache && configCache != 'null') {
      this.curConfig = JSON.parse(configCache)
    }
    this.loadMyDbs()
    this.getMyNameSql()
  },
  beforeDestroy() {
    // 在组件销毁前移除事件监听器
    document.removeEventListener('keydown', this.handleKeyDown);
  },

  methods: {
    dragControllerDiv() {
      var resize = document.getElementsByClassName('resize');
      var left = document.getElementsByClassName('left');
      var mid = document.getElementsByClassName('mid');
      var box = document.getElementsByClassName('box');
      console.log('drag')

      // window.addEventListener('mousemove', event => {
      //   console.log(`Mouse position: (${event.clientX}, ${event.clientY})`);
      // });

      for (let i = 0; i < resize.length; i++) {
        // 鼠标按下事件
        resize[i].onmousedown = function (e) {
          //颜色改变提醒
          // resize[i].style.background = '#818181';
          resize[i].style.background = 'rgb(55 187 240 / 81%)';
          var startX = e.clientX;
          // resize[i].left = resize[i].offsetLeft;
          resize[i].left = parseFloat(left[i].offsetWidth)
          console.log('resize[i].style.left', resize[i].left)

          // for (let j = 0; j < left.length; j++) {
          //   left[j].style.width = resize[i].left + 'px';
          // }

          // 鼠标拖动事件
          document.onmousemove = function (e) {
            var endX = e.clientX;
            var moveLen = resize[i].left + (endX - startX); // （endx-startx）=移动的距离。resize[i].left+移动的距离=左边区域最后的宽度
            // var moveLen = left[0].style.width + (endX - startX); // （endx-startx）=移动的距离。resize[i].left+移动的距离=左边区域最后的宽度
            // var moveLen = resize[i].offsetLeft + (endX - startX); // （endx-startx）=移动的距离。resize[i].left+移动的距离=左边区域最后的宽度
            console.log('moveLen', moveLen)
            var maxT = box[i].clientWidth - resize[i].offsetWidth; // 容器宽度 - 左边区域的宽度 = 右边区域的宽度
            if (moveLen < 200) moveLen = 200; // 左边区域的最小宽度为32px
            if (moveLen > maxT - 458) moveLen = maxT - 458; //右边区域最小宽度为150px

            console.log('moveLen2333', moveLen)

            // resize[i].style.left = moveLen; // 设置左侧区域的宽度
            console.log('resize[i].left  mmmm', resize[i].offsetLeft)


            for (let j = 0; j < left.length; j++) {
              left[j].style.width = moveLen + 'px';
              mid[j].style.width = box[0].offsetWidth - resize[i].offsetLeft + 'px';
            }
          };
          // 鼠标松开事件
          document.onmouseup = function (evt) {
            //颜色恢复
            resize[i].style.background = 'rgb(255 255 255 / 0%)';
            document.onmousemove = null;
            document.onmouseup = null;
            resize[i].releaseCapture && resize[i].releaseCapture(); //当你不在需要继续获得鼠标消息就要应该调用ReleaseCapture()释放掉
          };
          resize[i].setCapture && resize[i].setCapture(); //该函数在属于当前线程的指定窗口里设置鼠标捕获
          return false;
        };
      }
    },

    handleKeyDown(event) {
      // 检查按下的键是否是 Ctrl+S
      if (event.ctrlKey && event.key === 's') {
        event.preventDefault(); // 阻止浏览器默认保存行为
        // this.handleSave(); // 调用保存方法

        this.saveSql()
      }
    },
    //点击sql配置
    clickNameSql(item) {
      this.curConfig.dbSqlId = item.id
      this.curConfig.sqlName = item.sqlName
      this.curConfig.code = item.sql

      // this.$refs.editor[0].code=item.sql
      this.$refs.editor[0].setCodeContent(item.sql)
      // this.$set(this.$refs.editor[0], 'code', item.sql)
    },
    //获取我的sql储存
    getMyNameSql() {
      this.$http.get('/api/DbManager/GetMyDbSql').then(res => {

        this.myNameSqls = res.data
      })
    },
    //新增sql
    saveNewSql(){
      this.curConfig.dbSqlId=''
      this.saveSql()
    },
    saveSql() {
      this.$http
        .post("/api/DbManager/SaveSql", { id: this.curConfig.dbSqlId, sqlName: this.curConfig.sqlName, sql: this.curConfig.code })
        .then((res) => {

          if (res.succeeded) {
            this.curConfig.dbSqlId = res.data.id
            this.$message.success(res.data.msg)
            this.getMyNameSql()
          }
        });
    },
    getSelected() {
      this.isExecuted = true

      var curSelectionSql = this.$refs.editor[0].coder.getSelection()


      this.loading = true
      this.tableData = []
      //没有选择sql，执行全部
      if (curSelectionSql.length == 0) {
        curSelectionSql = this.curConfig.code
      }
      this.tableColumns = []
      this.$http
        .post("/api/DbManager/ExecuteSql", { id: this.curConfig.curSelect, DbName: this.curConfig.curDbName, sql: curSelectionSql })
        .then((res) => {
          this.loading = false

          if (!res.succeeded) {
            this.$message.error(res.errors)
          }
          this.tableData = JSON.parse(res.data.json)

          var columnsData = JSON.parse(res.data.columns)
          this.tableColumns = columnsData.map(x => ({ dbColumnName: x.ColumnName }))



          // this.tableList = res.data
        })

    },
    editorChange(val) {

      this.$set(this.curConfig, 'code', val)
      // .code = val
    },
    //清除已有数据库
    clearDb() {
      this.curConfig.curDbName = ''
      this.tableList = []
    },
    tableNameClick(item) {

      this.curTableName = item.name
      this.$http
        .post("/api/DbManager/GetTableDataList", {
          pageNumber: 1, pageSize: 15,
          searchKeyValues: [{ key: 'tableName', value: this.curTableName },
          { key: 'dbConfigId', value: this.curConfig.curSelect },
          { key: 'dbName', value: this.curConfig.curDbName },
          ]
        })
        .then((res) => {

          this.tableData = res.data.list
          this.tableColumns = res.data.allColumns


        })

    },
    //点击某一数据库
    dbNameClick(item) {

      //如果是重复点击，关闭数据库
      if (item == this.curConfig.curDbName) {
        this.clearDb();
        return
      }
      this.curConfig.curDbName = item
      this.getTables()
    },
    //获取数据库对应表
    getTables() {
      this.$http
        .post("/api/DbManager/GetCurTables", { id: this.curConfig.curSelect, DbName: this.curConfig.curDbName })
        .then((res) => {
          this.tableList = res.data
          this.$nextTick(() => {
            this.dragControllerDiv();

          })

        })
    },
    //查所有数据库
    getAllDbs() {
      this.$http
        .post("/api/DbManager/GetCurDbs", { id: this.curConfig.curSelect })
        .then((res) => {
          this.dbList = res.data
        })
    },
    //打开
    opOpenDbConfig() {
      this.dialogVisible = true
      this.$refs.dbConfigForm.show(this.currentSelectDbIndex)
    },
    //加载我的数据
    loadMyDbs() {
      this.$http
        .get("/api/DbManager/GetMyDbConfigs")
        .then((res) => {

          this.dbConfigData = res.data
          this.options = res.data.map(x => {
            return {
              value: x.id, label: x.connectName + `💦${x.dbName}`
            }
          })
        })
    },
  },
};
</script>
  
<style scoped>
/* 拖拽相关样式 */
/*包围div样式*/
.box {

  /* align-items: center; */
  display: flex;
}

/*左侧div样式*/
.left {
  width: calc(32% - 10px);
  /*左侧初始化宽度*/
  height: 100%;
  background: #FFFFFF;
  float: left;
}

/*拖拽区div样式*/
.resize {
  cursor: col-resize;
  position: relative;
  width: 5px;
  background-size: cover;
  color: white;
}

/*拖拽区鼠标悬停样式*/
.resize:hover {
  color: #444444;
}

/*右侧div'样式*/
.mid {
  /*右侧初始化宽度*/
  max-width: 80%;
  /* height: 100%; */
  background: #fff;
  /* box-shadow: -1px 4px 5px 3px rgba(0, 0, 0, 0.11); */
}


::v-deep ::-webkit-scrollbar {
  background-color: #efefef;
  width: 5px;
  height: 15px;
}

::v-deep ::-webkit-scrollbar-thumb {
  background-color: #0a0a0a66;
  outline: 1px solid slategrey;
  border-radius: 10px;

}

::v-deep ::-webkit-scrollbar-track {
  box-shadow: inset 0 0 5px grey;
}


.table-row {
  height: 40px;
  /* 设置行高为 40px */
  white-space: nowrap;
  /* 禁止换行 */
}

.tbNameItem {
  padding: 5px 0px;
  border-bottom: 1px dashed #bce2e1;
}

.fontRed {
  color: red;
  font-weight: 700;
}

.dbdiv {
  margin: 9px 7px;
  cursor: pointer;
}

.tagAllStyle {
  background-color: #447154;
  border-color: #e1f3d8;
  color: #ffffff !important;
}

.mixMode {
  /* mix-blend-mode: difference;
  color: white; */
  color: #022023;
}

.topTagContainer {
  margin-bottom: 15px;
  display: flex;
  align-items: center;
}

.contentInput img {
  width: 100px;
}

.tableSty {
  /* padding: 5px 11px; */
  width: 20%;
  height: 400px;
  overflow-y: auto;
  overflow-x: clip;
  color: black;
  font-weight: normal;
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

/* .leftTagContainer {
  width: 16%;
  position: fixed;
} */
.contentLine img {
  width: 50%;
}

.contentLine {
  line-height: 26px;
  white-space: pre-wrap;
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