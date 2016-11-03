library(ggplot2)
library(reshape2)
library(grid)
library(scales)

# set workspace
this.dir <- dirname(parent.frame(2)$ofile)
setwd(this.dir)
dirnames <- c('wiki-Vote', 'ca-AstroPh', 'com-dblp', 'com-lj')
discounts <- c('0.7', '0.85', '1')
ylows <- c(400, 400, 400, 2200, 2200, 2200, 5000, 5000, 5000, 60000, 60000, 60000)
yhighs <- c(700, 700, 700, 2800, 2800, 2800, 7000, 7000, 7000, 120000, 120000, 120000)

genfic <- function(dirname, discount, y_low, y_high) {
  # main procedure
  loc <- paste('./evaluation/(0.85,0.05)', dirname, sep='')
  
  fres <- paste(loc, '/Alpha=', discount, '/Fig5.txt', sep = '')
  dres <- read.csv(fres, sep=' ', stringsAsFactors=F, strip.white=TRUE)
  
  # dres$M <- factor(dres$M, levels = c('C', 'S'))
  ggplot(dres, aes(x=C, y=S)) +
    # scale_fill_manual(values=c("#CC6666", "#9999CC", "#66CC99")) +
    # scale_fill_manual(values=c('#6E548D', '#DB843D', '#C0504D')) + 
    theme_bw() +
    scale_y_continuous(limits=c(y_low, y_high))+
    scale_x_continuous(breaks=round(seq(0.05,1,by=0.05),1)) + 
    #   scale_y_log10(breaks=trans_breaks("log10", function(x) 10^x),
    #                 labels=trans_format("log10", math_format(10^.x)),
    #                 limits=c(1e3, 1e5)) + 
    geom_point(size=3) +
    geom_line(size=1) +
    xlab("Discount") +
    ylab("Influence Spread") +
    theme(axis.text.x = element_text(size=22),
          axis.text.y = element_text(size=22),
          axis.title.x = element_text(size=24),
          axis.title.y = element_text(size=24))
  figloc <- paste('./curve/curve_', 
                  dirname, '_', discount, '.pdf', sep='')
  ggsave(file=figloc, width = 12, height = 8, units = "in")
}

print('Generating Curve plots...')
for (i in 1:4) {
  for (j in 1:3) {
    ind <- (i-1) * 3 + j
    print(ind)
    time_unit <- "Time in seconds"
    if (i == 1) {
      time_unit <- "Time in milliseconds"
    }
    genfic(dirnames[i], discounts[j], ylows[ind], yhighs[ind])
  }
}
