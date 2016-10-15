import tushare as ts
import marshal, pickle

year = 2015

print(report_data)

report_data = ts.get_report_data(year, 4)
f = file('report_data', 'w')
pickle.dump(report_data, f)
f.close()

profit_data = ts.get_profit_data(year, 4)
f = file('profit_data', 'w')
pickle.dump(profit_data, f)
f.close()

growth_data = ts.get_growth_data(year, 4)
f = file('growth_data', 'w')
pickle.dump(growth_data, f)
f.close()
