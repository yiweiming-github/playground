import tushare as ts
import marshal, pickle

year = 2015
look_back = 1

profits_yoy_limit = 20
eps_yoy_limit = 20
roe_limit = 20
net_profit_ratio_limit = 20
mbrg_limit = 20

result = {}

for i in range(0, look_back):
    print('getting report data....')
    f =  file('report_data')
    report_data = pickle.load(f)
    f.close()    
    filter_report_data = report_data[report_data['profits_yoy'] > profits_yoy_limit]
    filter_report_data = filter_report_data[filter_report_data['eps_yoy'] > eps_yoy_limit]

    print(filter_report_data['code'].values)

    print('getting profit data....')
    f =  file('profit_data')
    profit_data = pickle.load(f)
    f.close()
    filter_profit_data = profit_data[profit_data['roe'] > roe_limit]
    filter_profit_data = filter_profit_data[filter_profit_data['net_profit_ratio'] > net_profit_ratio_limit]
    print(filter_profit_data['code'].values)

    print('getting growth data....')    
    f =  file('growth_data')
    growth_data = pickle.load(f)
    f.close()
    filter_growth_data = growth_data[growth_data['mbrg'] > mbrg_limit]
    print(filter_growth_data['code'].values)

    for code in filter_report_data['code'].values:
        if code in filter_profit_data['code'].values and code in filter_growth_data['code'].values:
            data_dict = {}
            data_dict['profits_yoy'] = filter_report_data[filter_report_data['code'] == code]['profits_yoy'].values[0]
            data_dict['roe'] = filter_profit_data[filter_profit_data['code'] == code]['roe'].values[0]
            data_dict['net_profit_ratio'] = filter_profit_data[filter_profit_data['code'] == code]['net_profit_ratio'].values[0]
            data_dict['mbrg'] = filter_growth_data[filter_growth_data['code'] == code]['mbrg'].values[0]
            data_dict['name'] = filter_growth_data[filter_growth_data['code'] == code]['name'].values[0]

            if not result.has_key(code):
                result[code] = []

            result[code].append(data_dict)

    year = year - 1

for (k, v) in result.items():
    if len(v) == look_back:
        print('%s %s - profits_yoy: %.2f, roe: %.2f, net_profit_ratio: %.2f, mbrg: %.2f' % (k, v[0]['name'], v[0]['profits_yoy'], v[0]['roe'], v[0]['net_profit_ratio'], v[0]['mbrg']))