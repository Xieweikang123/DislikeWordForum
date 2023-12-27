<template>
    <el-card class="cardContainer">
        <div class="titlediv">{{ title }}</div>
        <div class="rankingBar">
            <span v-for="colItem in cols">{{ colItem }}</span>
        </div>
        <div class="rowmargin">
            <!-- 加载骨架条 -->
            <el-skeleton :loading="isLoading" animated>
                <template slot="template">
                    <el-skeleton-item v-for="item in [0, 1, 2, 3]" :key="item" variant="text" style="margin-top: 20px" />
                </template>
            </el-skeleton>


            <span v-if="rdata.length == 0">今日暂无人背单词</span>
            <span v-else>
                <el-row class="disAlignCenter" style="    margin-top: 12px;" v-for="(item, index) in rdata"
                    :key="item.belongUserId">
                    <el-col class="disAlignCenter" :span="12" :offset="1">
                        <div style="position: relative; margin-right: 16px">
                            <el-avatar :src="getAvatorUrl(item.avatar)"> </el-avatar>
                            <span class="notxt">{{ index + 1 }}</span>
                        </div>
                        <span class="nickNames">
                            {{ item.nickName }}
                        </span>
                    </el-col>
                    <el-col :span="12"> {{ item.sum }} </el-col>
                </el-row>
            </span>
        </div>
    </el-card>
</template>

<script>
export default {
    props: ['title', 'cols', 'rdata'],
    data() {
        return {
            isLoading: false, // 示例，根据实际情况替换
            todayData: [], // 示例，根据实际情况替换
        };
    },
    methods: {
        //拼接头像前缀
        getAvatorUrl(url) {
            return process.env.VUE_APP_BASE_API + url;
        },

    },
};
</script>

<style>
.titlediv {
    font-size: 1.1em;
    font-weight: bold;
    color: #3f51b5c4;
    text-shadow: 2px 0px 4px #66d24a;
}

.notxt {
    font-size: 12px;
    position: absolute;
    bottom: 0;
    background: #fff3b3;
    font-weight: 900;
    width: 19px;
    border-radius: 12px;
    line-height: 19px;
    right: -11px;
    color: #914b4b;
}

.rankingBar {
    box-shadow: 2px 8px 11px 3px #f1fcff;
    padding: 13px 0;
    color: #82b1d3;
    font-weight: 600;
    display: flex;
    justify-content: space-around;
}


.rowmargin {
    margin-top: 10px;
    /* width: 41%; */
    margin: 13px auto;
    justify-content: space-between;
}

.cardContainer {
    text-align: center;
    width: 40%;
    margin: 0 10px;
    /* margin: 0 auto; */
}

.nickNames {
    white-space: nowrap;
    width: 100%;
    overflow: auto;
}

@media (max-width: 768px) {

    .cardContainer {
        margin-bottom: 23px;
        width: inherit;
    }

}
</style>