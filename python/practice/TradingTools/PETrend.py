import tushare as ts
import matplotlib
import matplotlib.pyplot as plt
from pylab import *
from datetime import datetime, timedelta

def get_report_date(date):
    year = date.date().year
    month = date.date().month    

    if month < 4:
        return (year - 1, 4)
    else:
        return (year, (month - 1) / 3)
        
def get_eps_series(code, dates):    
    eps_series = []    
    latest_year = 0
    latest_quarter = 0
    latest_eps = 0.0
    date_delta = timedelta(days = 1)
    for date in dates:
        (year, quarter) = get_report_date(date)
        if year == latest_year and quarter == latest_quarter:
            eps_series.append(latest_eps)
        else:
            print("Get profit data of %s, %s" % (year, quarter))
            quarter_eps = ts.get_profit_data(year, quarter)
            try:
                latest_eps = quarter_eps[quarter_eps['code'] == code]['eps'].values[0]
                #annualize eps
                latest_eps = latest_eps * 4.0 / quarter                
            except:
                print("failed to get eps, using last one %s" % (latest_eps))
            eps_series.append(latest_eps)
            latest_year = year
            latest_quarter = quarter
        
        print('%s: %s' % (date, latest_eps))
        date += date_delta
    return eps_series



code = '000651'

#data = ts.get_hist_data(code, start='2012-01-01', end='2016-06-30')
data = ts.get_h_data(code, autype=None, start='2006-01-01', end='2016-01-31')
print('Get price data done!')

date_index = [];
for str in data.index:
    date_index.append(str.to_datetime())

fig = plt.figure()
ax = fig.add_subplot(111)
ax.plot(date_index, data['close'].values)

print('start: %s, end: %s' % (date_index[0], date_index[-1]))
eps = get_eps_series(code, date_index)
print(eps)
ax.plot(date_index, eps)

pe =  data['close'].values / eps
ax.plot(date_index, pe)

plt.show()