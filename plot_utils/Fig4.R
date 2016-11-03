library(ggplot2)
library(reshape2)
library(grid)
library(scales)

# set workspace
this.dir <- dirname(parent.frame(2)$ofile)
setwd(this.dir)
dirnames <- c('wiki-Vote', 'ca-AstroPh', 'com-dblp', 'com-lj')
discounts <- c('0.7', '0.85', '1')

# main procedure
genfic <- function(dirname, discount) {
  loc <- paste('./evaluation/', dirname, sep='')
  
  fres <- paste(loc, '/Alpha=', discount, '/Fig4.txt', sep = '')
  dres <- read.csv(fres, sep=' ', stringsAsFactors=F, strip.white=TRUE)
  
  idseq <- c(seq(10, 50, 10))
  topline <- c(0.63, 0.63, 0.63, 0.63)
  
  ggplot(dres, aes(x=B, y=AccuracyLB)) +
    theme_bw() +
    scale_y_continuous(labels=percent, limits=c(0.5,0.65)) + 
    geom_point() +
    geom_line() +
    geom_hline(aes(yintercept=0.63), color='#009E73', size=1) +
    geom_text(aes(10, 0.63, label="Approximation Upper Bound (63%)"), hjust=-0.35, vjust=-1, size=6.5) + 
    xlab("Budget") +
    ylab("Approximation Lower Bound") +
    theme(
      legend.text=element_text(size=14),
      axis.text=element_text(size=15),
      axis.title=element_text(size=18),
      plot.title=element_text(size=18)
    ) 
  # ggtitle(bquote(.(dirname) ~ 'with' ~ alpha ~ '=' ~ .(discount)))
  # paste("wiki-Vote with ", alpha, "=1"))
  figloc <- paste('./imquality/', 
                  dirname, '_', discount, '.pdf', sep='')
  ggsave(file=figloc, width = 12, height = 8, units = "in")
}

print('Generating IMQuality plots...')
for (i in 1:4) {
  for (j in 1:3) {
    ind <- (i-1) * 3 + j
    print(ind)
    genfic(dirnames[i], discounts[j])
  }
}
