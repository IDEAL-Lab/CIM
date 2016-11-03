library(ggplot2)
library(reshape2)
library(grid)
library(scales)

# set workspace
this.dir <- dirname(parent.frame(2)$ofile)
setwd(this.dir)
dirnames <- c('wiki-Vote', 'ca-AstroPh', 'com-dblp', 'com-lj')
discounts <- c('0.7', '0.85', '1')
ylows <- c(1e0, 1e0, 1e0, 1e0, 1e0, 1e1, 1e1, 1e1, 1e1, 1e2, 1e2, 1e3)
yhighs <- c(1e6, 1e6, 1e6, 1e2, 1e3, 1e3, 1e3, 1e3, 1e3, 1e4, 1e5, 1e5)

genfic <- function(dirname, discount, y_low, y_high, time_unit) {
  # main procedure
  loc <- paste('./evaluation(0.85,0.05)/', dirname, sep='')
  
  fres <- paste(loc, '/Alpha=', discount, '/Fig6.txt', sep = '')
  dres <- read.csv(fres, sep=' ', stringsAsFactors=F, strip.white=TRUE)
  
  dres$M <- factor(dres$M, levels = c('CD', 'UC', 'IM', 'GBT'))
  ggplot(dres, aes(x=B, y=TIME/500, color=M)) +
    # scale_fill_manual(values=c("#CC6666", "#9999CC", "#66CC99")) +
    # scale_fill_manual(values=c('#6E548D', '#DB843D', '#C0504D')) + 
    theme_bw() +
    scale_y_log10(breaks=trans_breaks("log10", function(x) 10^x),
                  labels=trans_format("log10", math_format(10^.x)),
                  limits=c(y_low, y_high)) + 
    geom_point(aes(shape=M), size=3) +
    geom_line(size=1) +
    xlab("Budget") +
    ylab(time_unit) +
    theme(
      legend.position = c(0.1, 0.8), # c(0,0) bottom left, c(1,1) top-right.
      legend.title=element_blank(),
      # legend.key.width=unit(0.6, "cm"),
      legend.text=element_text(size=9)
    ) +
    ggtitle(bquote(.(dirname) ~ 'with' ~ alpha ~ '=' ~ .(discount)))
  # paste("wiki-Vote with ", alpha, "=1"))
  figloc <- paste('./time/time_', 
                  dirname, '_', discount, '.pdf', sep='')
  ggsave(file=figloc, width = 12, height = 8, units = "in")
}

print('Generating Time plots...')
for (i in 1:4) {
  for (j in 1:3) {
    ind <- (i-1) * 3 + j
    print(ind)
    time_unit <- "Time in seconds"
    if (i == 1) {
      time_unit <- "Time in milliseconds"
    }
    genfic(dirnames[i], discounts[j], ylows[ind], yhighs[ind], time_unit)
  }
}
