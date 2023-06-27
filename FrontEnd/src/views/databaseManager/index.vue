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
        <div v-if="itemDbName == curConfig.curDbName" style="display: flex;">
          <div style="    padding: 5px 11px;     width: 20%;   color: black;    font-weight: normal;">
            <div class="tbNameItem" :class="{ 'fontRed': curTableName == item.name }" v-for="item in tableList"
              @click.stop="tableNameClick(item)">
              {{ item.name }}<span v-if="item.description"> ({{ item.description }})</span>
            </div>
          </div>
          <div style="width:100%;">
            <el-button :loading="loading" style="margin-bottom: 5px;" @click="getSelected()">▶️执行</el-button>

            <CommonEditor ref="editor" :value="curConfig.code" language="sql" @change="editorChange"
              style=" width: 83%;height: 200px">
            </CommonEditor>
            <span v-if="isExecuted">共 {{ tableData.length }}</span>
            <!-- {{ curTableName }} -->
            <el-table row-class-name="table-row" v-loading="loading" :row-style="{ height: '80px' }" :data="tableData"
              style="width: 100%">
              <!-- <el-table-column show-overflow-tooltip prop="id" label="id" width="180">
              </el-table-column> -->
              <el-table-column v-for="item in tableColumns" show-overflow-tooltip :prop="item.dbColumnName"
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
      isExecuted:false,
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
      curConfig: {
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
    //表切换
    curTableName() {

    },
    curConfig: {
      handler: function (newVal, oldVal) {
        console.log('watch curConfig', newVal);
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
      console.log('watch select', this.curConfig.curSelect)
      // this.clearDb();
      //查所有数据库
      this.getAllDbs()
    },
  },
  mounted() {
    // console.log('Ace editor instance:', this.$refs.aceEditor.editor);

    var configCache = localStorage.getItem('curConfig')
    if (configCache && configCache != 'null') {
      this.curConfig = JSON.parse(configCache)
    }
    this.loadMyDbs()
  },

  methods: {
    getSelected() {
      this.isExecuted=true
      console.log('sss', this.$refs.editor)
      var curSelectionSql = this.$refs.editor[0].coder.getSelection()
      console.log('curSelectionSql', curSelectionSql)
      console.log('curSelectionSql', curSelectionSql.length)
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
          console.log('ExecuteSql', res)
          if (!res.succeeded) {
            this.$message.error(res.errors)
          }
          this.tableData = JSON.parse(res.data.json)
          console.log('ExecuteSql tableData', this.tableData)
          var columnsData = JSON.parse(res.data.columns)
          this.tableColumns = columnsData.map(x => ({ dbColumnName: x.ColumnName }))

          console.log('ExecuteSql tableColumns', this.tableColumns)

          // this.tableList = res.data
        })

    },
    editorChange(val) {
      console.log('editorChange', val)
      this.$set(this.curConfig, 'code', val)
      // .code = val
    },
    //清除已有数据库
    clearDb() {
      this.curConfig.curDbName = ''
      this.tableList = []
    },
    tableNameClick(item) {
      console.log('tableNameClick', item)
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
          console.log('GetTableDataList', res)
          this.tableData = res.data.list
          this.tableColumns = res.data.allColumns


        })

    },
    //点击某一数据库
    dbNameClick(item) {
      console.log('dbNameClick', item)
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
          console.log('GetCurTables', res)
          this.tableList = res.data
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
          console.log('get GetMyDbConfigs', res)
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
  margin: 18px 7px;
  cursor: pointer;
}

.dbContainer {
  /* width: fit-content; */
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