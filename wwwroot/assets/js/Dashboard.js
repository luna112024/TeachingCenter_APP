let config = {
    colors: {
        primary: '#696cff',
        secondary: '#8592a3',
        success: '#71dd37',
        info: '#03c3ec',
        warning: '#ffab00',
        danger: '#ff3e1d',
        dark: '#233446',
        black: '#000',
        white: '#fff',
        body: '#f4f5fb',
        headingColor: '#566a7f',
        axisColor: '#a1acb8',
        borderColor: '#eceef1'
    },
    colors_label: {
        primary: "#666ee81a",
        secondary: "#8897aa1a",
        success: "#28d0941a",
        info: "#1e9ff21a",
        warning: "#ff91491a",
        danger: "#ff49611a",
        dark: "#181c211a",
    },
    colors_dark: {
        cardColor: "#2b2c40",
        bodyBg: "#232333",
        bodyColor: "#a3a4cc",
        headingColor: "#cbcbe2",
        textMuted: "#7071a4",
        borderColor: "#444564",
    }
};
// Start GenderPopulationChart Chart
function LoadStaticticProvince() {
    var provinceCode = $('#provinceDropdownDash').val();
    var districtCode = $('#districtDropdownDash').val();
    var communeCode = $('#communeDropdownDash').val();
    var villageCode = $('#villageDropdownDash').val();
    $.ajax({
        url: '/home/villagePopulation',
        type: 'GET',
        data: {
            provinceCode: provinceCode,
            districtCode: districtCode,
            communeCode: communeCode,
            villageCode: villageCode
        },
        dataType: "JSON",
        success: function (data) {
            $("#totalPeopleVillage").text(data.familyHead);
            $("#IdCardExpire").text(data.idCardExpire);
            $("#nearVotingAge").text(data.nearVotingAge);
            $("#peopleWorkGov").text(data.peopleWorkGov);
            $("#villageByAddress").text(data.villageTotal);
            $("#headOfFamily").text(data.familyHead);
            $("#memberPartyTotal").text(data.memberTotalParty);
            $("#newMemberThisYear").text(data.newMemberThisYear);
            $("#peopleHasVoted").text(data.peopleHasVoted);
            $("#peopleNotVote").text(data.peopleNotVoted);

            


            ApexCharts.exec('mychart', 'updateSeries', [{
                data: data.series
            }], true);

            ApexCharts.exec('gender', 'updateSeries', 
                 data.seriesGender
                , true);
            ApexCharts.exec('party', 'updateSeries',
                data.seriesParty
                , true);

        }
    });
}
$(function () {


    let e, t;
    t = (
        config.isDarkStyle
            ? ((e = config.colors_dark.textMuted), config.colors_dark)
            : ((e = config.colors.textMuted), config.colors)
    ).headingColor;
    var s = {
        donut: {
            series1: config.colors.info,
        },
        line: {
            series1: config.colors.success,
        },
    },
        a = document.querySelector("#CommunePopulation"),
        r = {
           
            series: [
                {
                    name: "",
                    type: "column",
                    data: [0, 0, 0, 0],
                }
            ],
            chart: {
                id: 'mychart',
                height: 370,
                type: "bar",
                stacked: !1,
                parentHeightOffset: 0,
                toolbar: { show: !1 },
                zoom: { enabled: !1 },
            },
            markers: {
                size: 4,
                colors: [config.colors.white],
                strokeColors: s.line.series2,
                hover: { size: 6 },
                borderRadius: 4,
            },
            stroke: { curve: "smooth", width: [0, 3], lineCap: "round" },
            legend: {
                show: !0,
                position: "bottom",
                markers: { width: 8, height: 8, offsetX: -3 },
                height: 40,
                offsetY: 10,
                itemMargin: { horizontal: 10, vertical: 0 },
                fontSize: "15px",
                fontFamily: "Khmer OS Siemreap",
                fontWeight: 400,
                labels: { colors: t, useSeriesColors: !1 },
                offsetY: 10,
            },
            grid: { strokeDashArray: 8 },
            colors: [s.line.series1, s.line.series2],
            fill: { opacity: [1, 1] },
            plotOptions: {
                bar: {
                    columnWidth: "30%",
                    startingShape: "rounded",
                    endingShape: "rounded",
                    borderRadius: 4,
                },
            },
            dataLabels: { enabled: !1 },
            xaxis: {
                tickAmount: 4,
                categories: [
                    "ចំនួនប្រជាពលរដ្ឋ",
                    "មានអត្តសញ្ញាណប័ណ្ណ",
                    "មានប័ណ្ណសមាជិកបក្ស",
                    "មានឈ្មោះបោះឆ្នោត"
                ],
                labels: {
                    style: {
                        colors: e,
                        fontSize: "10px",
                        fontFamily: "Khmer OS Siemreap",
                        fontWeight: 400,
                    },
                },
                axisBorder: { show: !1 },
                axisTicks: { show: !1 },
            },
            yaxis: {
                forceNiceScale: true,
                tickAmount: 5,
                min: 0,
                labels: {
                    style: {
                        colors: e,
                        fontSize: "13px",
                        fontFamily: "Khmer OS Siemreap",
                        fontWeight: 400,
                    },
                    formatter: function (e) {
                        return e.toFixed(0) + " នាក់";
                    },
                },
            },
            responsive: [
                {
                    breakpoint: 1400,
                    options: {
                        chart: { height: 370 },
                        xaxis: { labels: { style: { fontSize: "10px" } } },
                        legend: {
                            itemMargin: { vertical: 0, horizontal: 10 },
                            fontSize: "13px",
                            offsetY: 12,
                        },
                    },
                },
                {
                    breakpoint: 1399,
                    options: {
                        chart: { height: 415 },
                        plotOptions: { bar: { columnWidth: "50%" } },
                    },
                },
                {
                    breakpoint: 982,
                    options: { plotOptions: { bar: { columnWidth: "30%" } } },
                },
                {
                    breakpoint: 480,
                    options: { chart: { height: 250 }, legend: { offsetY: 7 } },
                },
            ],
        };
        //charts.render();
    let chart = new ApexCharts(a, r).render();
   
});

let cardColor, headingColor, axisColor, shadeColor, borderColor;
cardColor = config.colors.white;
headingColor = config.colors.headingColor;
axisColor = config.colors.axisColor;
borderColor = config.colors.borderColor;



const chartOrderStatistics = document.querySelector('#PopulationChart'),
    orderChartConfig = {
        chart: {
            id: 'gender',
            height: 165, 
            width: 130, 
            type: 'donut'
        },
        fontFamily: 'Khmer OS Siemreap',
        labels: ['ភេទប្រុស', 'ភេទស្រី', 'ព្រះសង្ឃ'],
        series: [0, 0, 0],
        colors: [config.colors.info, config.colors.success, config.colors.warning],
        stroke: {
            width: 5,
            colors: [cardColor]
        },
        dataLabels: {
            enabled: false,
            formatter: function (val, opt) {
                return parseInt(val);
            }
        },
        legend: {
            show: false
        },
        plotOptions: {
            pie: {
                donut: {
                    size: '75%',
                    labels: {
                        show: true,
                        value: {
                            fontSize: '1.5rem',
                            fontFamily: 'Khmer OS Siemreap',
                            color: headingColor,
                            offsetY: -15,
                            formatter: function (val) {
                                return parseInt(val);
                            }
                        },
                        name: {
                            offsetY: 20,
                            fontFamily: 'Khmer OS Siemreap'
                        },
                        total: {
                            show: true,
                            fontSize: '0.8125rem',
                            color: axisColor,
                            label: 'សរុប',
                            fontFamily: 'Khmer OS Siemreap'
                        }
                    }
                }
            }
        }
    };
if (typeof chartOrderStatistics !== undefined && chartOrderStatistics !== null) {
    const statisticsChart = new ApexCharts(chartOrderStatistics, orderChartConfig);
    statisticsChart.render();
}
//End GenderPopulationChart Chart


var o = config.colors.white,
    m = config.colors.headingColor,
    r = config.colors.borderColor,
    j = document.querySelector("#growthChart"),
    k = {
        series: [0],
        labels: ["បក្ស / ប្រជាពលរដ្ឋ"],
        chart: { height: 200, type: "radialBar", id: 'party' },
        plotOptions: {
            radialBar: {
                size: 100,
                offsetY: 10,
                startAngle: -150,
                endAngle: 150,
                hollow: { size: "55%" },
                track: { background: o, strokeWidth: "100%" },
                dataLabels: {
                    name: {
                        offsetY: 15,
                        color: m,
                        fontSize: "11px",
                        fontWeight: "500",
                        fontFamily: "Khmer OS Siemreap",
                    },
                    value: {
                        offsetY: -25,
                        color: m,
                        fontSize: "22px",
                        fontWeight: "500",
                        fontFamily: "Khmer OS Siemreap",
                    },
                },
            },
        },
        colors: [config.colors.primary],
        fill: {
            type: "gradient",
            gradient: {
                shade: "dark",
                shadeIntensity: 0.5,
                gradientToColors: [config.colors.primary],
                inverseColors: !0,
                opacityFrom: 1,
                opacityTo: 0.6,
                stops: [30, 70, 100],
            },
        },
        stroke: { dashArray: 5 },
        grid: { padding: { top: -35, bottom: -10 } },
        states: {
            hover: { filter: { type: "none" } },
            active: { filter: { type: "none" } },
        },

    }
null !== j && new ApexCharts(j, k).render();

//Population compare to Voter Chart