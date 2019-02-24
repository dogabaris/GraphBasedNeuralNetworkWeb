import { Component, ElementRef, OnInit  } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  title = 'frontend-app';

  constructor(private elementRef: ElementRef) {
    //this.path = this.elementRef.nativeElement.getAttribute('path');
  }


  public ngOnInit() {
    //console.log('ecco il path da caricare', this.path);
  }
}
