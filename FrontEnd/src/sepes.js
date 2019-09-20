export default {
    getSupplierList: () => ["Tom Andre", "Bjørn Kristiansen", "Ole Martin", "Kristoffer Årdal"],

    getSponsorList: () => ["Tom Andre", "Bjørn Kristiansen", "Ole Martin", "Kristoffer Årdal"],

    getDatasetList: () => ["Gudrun", "Heidrun", "Vale", "Volve", "Snøhvit", "Troll", "Sleipner", "Gullfaks", "Goliat", "Kvitebjørn"], 

    getData: async () => {
        const data = await fetch("https://localhost:5001/api/study/list");
        return await data.json();
    }
}