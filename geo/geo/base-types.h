#pragma once


#ifndef BASE
#define BASE
#include "Shape.h"

class point_t
{
public:
	point_t() : x_(0), y_(0) {}
	point_t(double x, double y) : x_(x), y_(y) {}
	point_t(const point_t& ob) : x_(ob.x_), y_(ob.y_) {}
	~point_t() {};

	void setXY(double x, double y) { x_ = x; y_ = y; }
	void setX(double x) { x_ = x; }
	void setY(double y) { y_ = y; }
	double getX() const { return x_; }
	double getY() const { return y_; }
private:
	double x_;
	double y_;
};

class rectangle :public Shape
{ 
public:
	rectangle(point_t center, double len, double height) : center_(point_t(center)), len_(len), height_(height) {};
	rectangle(double x1, double y1, double x2, double y2) : center_(point_t((x1 + x2) / 2, (y1 + y2) / 2)), len_(abs(x1 - x2)), height_(abs(y1 - y2)) {};
	rectangle(const rectangle& source) : center_(point_t(source.center_)), len_(source.len_), height_(source.height_) {};
	~rectangle() {};
	double getArea();
	rectangle& clone();
	rectangle& getFrameRect();
	void move(double delta_x, double delta_y);
	void moveTo(double x, double y);
	void scale(double x, double y, double sigma);
	std::string getName();
	double getMaxWidth();
	double getMaxHeigth();
	double getPoints(int index);
	point_t getCenter() { return center_; }
	double getX() { return center_.getX(); };
	double getY() { return center_.getY(); };
private:
	point_t center_;
	double len_;
	double height_;
};

#endif // !BASE