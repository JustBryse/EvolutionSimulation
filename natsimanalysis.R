#load libraries
library(dplyr)
library(readr)
library(ggplot2)
library(plot3D)
library(plot3Drgl)

#choose simData.txt to be read. An example file is attached in this branch. If a new simulation is run, choose the simData.txt file in the 'Assets' folder (BryseBranch)
natsim_data <- read.table(file=file.choose(),
                         header=F, sep="/",
                         na.strings="null",
                         stringsAsFactors=F)

#update column labels
natsim_data <- natsim_data %>%
  rename(gen=V1, id=V2, speed=V3, awareness=V4)

#clean the data
dist_data <- natsim_data %>%
  select(-gen) %>%
  group_by(id) %>%
  mutate(lifespan = n())
dist_data <- distinct(dist_data, id, .keep_all=T)

#awareness vs speed graph
vis <- ggplot(data = dist_data, aes(x=awareness, y=speed)) + geom_point(aes(color=id)) + geom_smooth()
vis

#speed vs lifespan graph
lvis <- ggplot(data = dist_data, aes(x=speed, y=lifespan)) + geom_point(aes(color=id)) + geom_smooth()
lvis

#awareness vs lifespan graph
ldvis <- ggplot(data = dist_data, aes(x=awareness, y=lifespan)) + geom_point(aes(color=id)) + geom_smooth()
ldvis

#awareness(x) vs speed(y) vs lifespan(z)
scatter3D(dist_data$awareness,dist_data$speed,dist_data$lifespan, phi = 30, theta = 45, ticktype = 'detailed', type='h', xlab='awareness',ylab='speed',zlab='lifespan')

#interactive 3D render of the scatter3D graph from above
plotrgl()
